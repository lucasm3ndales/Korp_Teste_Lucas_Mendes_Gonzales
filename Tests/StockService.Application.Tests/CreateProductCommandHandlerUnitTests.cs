using Mapster;
using Moq;
using StockService.Application.Common.Exceptions;
using StockService.Application.Repositories;
using StockService.Application.UseCases.CreateProduct;
using StockService.Domain.Entities;
using StockService.Domain.Exceptions;

namespace StockService.Application.Tests;

public class CreateProductCommandHandlerUnitTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly CreateProductCommandHandlerUnit _handlerUnit;

    public CreateProductCommandHandlerUnitTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _handlerUnit = new CreateProductCommandHandlerUnit(_productRepositoryMock.Object);
        TypeAdapterConfig.GlobalSettings.Scan(typeof(CreateProductCommandHandlerUnit).Assembly);
    }

    [Fact]
    public async Task Should_ReturnSuccess_When_CodeIsUnique()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Code = "HCL-123",
            Description = "Café",
            InitialStockBalance = 10
        };

        _productRepositoryMock
            .Setup(r =>
                r.GetByCode(
                    command.Code,
                    It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((Product)null!);

        // Act
        var result = await _handlerUnit.Handle(command, CancellationToken.None);

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

    [Fact]
    public async Task Should_Throw_ProductCodeAlreadyExistsException_When_CodeAlreadyExists()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Code = "HCL-123",
            Description = "Café",
            InitialStockBalance = 10
        };

        var existingProduct = new Product("HCL-123", "Café", 5);

        _productRepositoryMock.Setup(r => r.GetByCode(
                command.Code,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);

        // Act & Assert
        await Assert.ThrowsAsync<ProductCodeAlreadyExistsException>(() =>
            _handlerUnit.Handle(command, CancellationToken.None));

        _productRepositoryMock.Verify(r => r.Add(
            It.IsAny<Product>(),
            It.IsAny<CancellationToken>()), Times.Never);

        _productRepositoryMock.Verify(r => r.SaveChanges(
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Should_Throw_DomainException_When_ProductCreationFails()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Code = "HCL-123",
            Description = "Café",
            InitialStockBalance = -1
        };

        _productRepositoryMock.Setup(r => r.GetByCode(
                command.Code,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidProductStockBalanceException>(() =>
            _handlerUnit.Handle(command, CancellationToken.None));

        _productRepositoryMock.Verify(r => r.Add(
            It.IsAny<Product>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }
}