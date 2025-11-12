using Mapster;
using Moq;
using StockService.Application.Common.Exceptions;
using StockService.Application.Common.Repositories;
using StockService.Application.UseCases.CreateProduct;
using StockService.Domain.Entities;
using StockService.Domain.Exceptions;

namespace StockService.Application.Unit.Tests;

public class CreateProductCommandHandlerUnitTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerUnitTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _handler = new CreateProductCommandHandler(_productRepositoryMock.Object);
    }

    [Fact(DisplayName = "Deve retornar sucesso quando o código for único")]
    public async Task Should_ReturnSuccess_When_CodeIsUnique()
    {
        // Arrange
        var command = new CreateProductCommand("HCL-123", "Café", 10);

        _productRepositoryMock
            .Setup(r =>
                r.GetByCode(
                    command.Code,
                    It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((Product)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(command.Code, result.Data.Code);

        _productRepositoryMock.Verify(r => r.Add(
            It.Is<Product>(p => p.Code == command.Code),
            It.IsAny<CancellationToken>()), Times.Once);

        _productRepositoryMock.Verify(r => r.SaveChanges(
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Deve lançar ProductCodeAlreadyExistsException quando o código já existir")]
    public async Task Should_Throw_ProductCodeAlreadyExistsException_When_CodeAlreadyExists()
    {
        // Arrange
        var command = new CreateProductCommand("HCL-123", "Café", 10);

        var existingProduct = new Product("HCL-123", "Café", 5);

        _productRepositoryMock.Setup(r => r.GetByCode(
                command.Code,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);

        // Act & Assert
        await Assert.ThrowsAsync<ProductCodeAlreadyExistsException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _productRepositoryMock.Verify(r => r.Add(
            It.IsAny<Product>(),
            It.IsAny<CancellationToken>()), Times.Never);

        _productRepositoryMock.Verify(r => r.SaveChanges(
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Deve lançar DomainException quando a criação do produto falhar (ex: saldo negativo)")]
    public async Task Should_Throw_DomainException_When_ProductCreationFails()
    {
        // Arrange
        var command = new CreateProductCommand("HCL-123", "Café", -1);


        _productRepositoryMock.Setup(r => r.GetByCode(
                command.Code,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidProductStockBalanceException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _productRepositoryMock.Verify(r => r.Add(
            It.IsAny<Product>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact(DisplayName = "Deve lançar DomainException quando o Código for inválido")]
    public async Task Should_Throw_DomainException_When_CodeIsInvalid()
    {
        // Arrange
        var command = new CreateProductCommand("", "Café", 10);

        _productRepositoryMock.Setup(r => r.GetByCode(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product)null!);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidProductCodeException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _productRepositoryMock.Verify(r => r.Add(
            It.IsAny<Product>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }
}