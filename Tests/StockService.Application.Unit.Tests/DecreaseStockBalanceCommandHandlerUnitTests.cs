using Microsoft.EntityFrameworkCore;
using Moq;
using StockService.Application.Common.Exceptions;
using StockService.Application.Repositories;
using StockService.Application.UseCases.DecreaseStockBalance;
using StockService.Domain.Entities;
using StockService.Domain.Exceptions;
using StockService.Domain.ValueObjects;

namespace StockService.Application.Unit.Tests;

public class DecreaseStockBalanceCommandHandlerUnitTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly DecreaseStockBalanceCommandHandler _handler;

    public DecreaseStockBalanceCommandHandlerUnitTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _handler = new DecreaseStockBalanceCommandHandler(_productRepositoryMock.Object);
    }

    [Fact(DisplayName = "Deve retornar sucesso quando o produto existe e o estoque é suficiente")]
    public async Task Should_ReturnSuccess_When_ProductExistsAndStockIsSufficient()
    {
        // Arrange
        var product = new Product("HCL-123", "Café", 10);
        var command = new DecreaseStockBalanceCommand 
        { 
            Id = product.Id, 
            QuantityUsed = 5 
        };

        _productRepositoryMock.Setup(r => r.GetById(
                product.Id, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(5, product.StockBalance);
        
        _productRepositoryMock.Verify(r => r.SaveChanges(
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Deve lançar ProductNotExistsException quando o produto não for encontrado")]
    public async Task Should_Throw_ProductNotExistsException_When_ProductIsNotFound()
    {
        // Arrange
        var command = new DecreaseStockBalanceCommand 
        { 
            Id = ProductId.NewId(), 
            QuantityUsed = 5 
        };

        _productRepositoryMock.Setup(r => r.GetById(
                command.Id, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product)null!);

        // Act & Assert
        await Assert.ThrowsAsync<ProductNotExistsException>(() => 
            _handler.Handle(command, CancellationToken.None));
            
        _productRepositoryMock.Verify(r => r.SaveChanges(
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Deve lançar InsufficientStockBalanceException quando o domínio rejeitar (estoque insuficiente)")]
    public async Task Should_Throw_InsufficientStockBalanceException_When_DomainThrowsIt()
    {
        // Arrange
        var product = new Product("HCL-123", "Café", 5);
        var command = new DecreaseStockBalanceCommand 
        { 
            Id = product.Id, 
            QuantityUsed = 10 
        };

        _productRepositoryMock.Setup(r => r.GetById(
                product.Id, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act & Assert
        await Assert.ThrowsAsync<InsufficientStockBalanceException>(() => 
            _handler.Handle(command, CancellationToken.None));
            
        _productRepositoryMock.Verify(r => r.SaveChanges(
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Deve lançar StockConcurrencyException quando ocorrer um DbUpdateConcurrencyException")]
    public async Task Should_Throw_StockConcurrencyException_When_DbUpdateConcurrencyOccurs()
    {
        // Arrange
        var product = new Product("HCL-123", "Café", 10);
        
        var command = new DecreaseStockBalanceCommand 
        { 
            Id = product.Id, 
            QuantityUsed = 5 
        };

        _productRepositoryMock.Setup(r => r.GetById(
                product.Id, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _productRepositoryMock.Setup(r => r.SaveChanges(
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateConcurrencyException());

        // Act & Assert
        await Assert.ThrowsAsync<StockConcurrencyException>(() => 
            _handler.Handle(command, CancellationToken.None));
    }
}