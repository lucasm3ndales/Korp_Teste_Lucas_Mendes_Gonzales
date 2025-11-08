using Microsoft.EntityFrameworkCore;
using Moq;
using StockService.Application.Common.Exceptions;
using StockService.Application.Repositories;
using StockService.Application.UseCases.DecreaseStockBalance;
using StockService.Domain.Entities;
using StockService.Domain.Exceptions;
using StockService.Domain.ValueObjects;

namespace StockService.Application.Unit.Tests;

public class DecreaseStockProductsInBatchCommandHandlerUnitTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly DecreaseStockProductsInBatchCommandHandler _handler;

    public DecreaseStockProductsInBatchCommandHandlerUnitTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _handler = new DecreaseStockProductsInBatchCommandHandler(_productRepositoryMock.Object);
    }

    [Fact(DisplayName = "Deve retornar sucesso quando todos os produtos existem e têm estoque suficiente")]
    public async Task Should_ReturnSuccess_When_AllProductsExistAndHaveSufficientStock()
    {
        // Arrange
        var productA = new Product("HCL-123", "Café", 10);
        var productB = new Product("HCL-456", "Açúcar", 20);
        var products = new List<Product> { productA, productB };

        var command = new DecreaseStockProductsInBatchCommand
        {
            Items =
            [
                new StockProductItemCommand { Id = productA.Id, QuantityUsed = 5 },
                new StockProductItemCommand { Id = productB.Id, QuantityUsed = 10 }
            ]
        };

        _productRepositoryMock.Setup(r => r.GetProductsByIds(
                It.IsAny<List<ProductId>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(5, productA.StockBalance);
        Assert.Equal(10, productB.StockBalance);

        _productRepositoryMock.Verify(r => r.SaveChanges(
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Deve lançar ProductNotExistsException quando UM produto não for encontrado")]
    public async Task Should_Throw_ProductNotExistsException_When_OneProductIsNotFound()
    {
        // Arrange
        var productA = new Product("HCL-123", "Café", 10);
        var notFoundProductId = ProductId.NewId(); // Um ID que não existe
        
        var command = new DecreaseStockProductsInBatchCommand
        {
            Items =
            [
                new StockProductItemCommand { Id = productA.Id, QuantityUsed = 5 },
                new StockProductItemCommand { Id = notFoundProductId, QuantityUsed = 5 }
            ]
        };

        _productRepositoryMock.Setup(r => r.GetProductsByIds(
                It.IsAny<List<ProductId>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product> { productA });

        // Act & Assert
        await Assert.ThrowsAsync<ProductNotExistsException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _productRepositoryMock.Verify(r => r.SaveChanges(
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Deve lançar InsufficientStockBalanceException quando UM produto não tiver estoque")]
    public async Task Should_Throw_InsufficientStockBalanceException_When_OneProductLacksStock()
    {
        // Arrange
        var productA = new Product("HCL-123", "Café", 10); // Tem estoque
        var productB = new Product("HCL-456", "Açúcar", 5); // NÃO tem estoque
        var products = new List<Product> { productA, productB };

        var command = new DecreaseStockProductsInBatchCommand
        {
            Items =
            [
                new StockProductItemCommand { Id = productA.Id, QuantityUsed = 5 }, // OK
                new StockProductItemCommand { Id = productB.Id, QuantityUsed = 10 } // Falha
            ]
        };

        _productRepositoryMock.Setup(r => r.GetProductsByIds(
                It.IsAny<List<ProductId>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        // Act & Assert
        await Assert.ThrowsAsync<InsufficientStockBalanceException>(() =>
            _handler.Handle(command, CancellationToken.None));
        
        _productRepositoryMock.Verify(r => r.SaveChanges(
            It.IsAny<CancellationToken>()), Times.Never);
            
        Assert.Equal(10, productA.StockBalance);
    }

    [Fact(DisplayName = "Deve lançar StockConcurrencyException quando ocorrer um DbUpdateConcurrencyException")]
    public async Task Should_Throw_StockConcurrencyException_When_DbUpdateConcurrencyOccurs()
    {
        // Arrange
        var productA = new Product("HCL-123", "Café", 10);
        var products = new List<Product> { productA };

        var command = new DecreaseStockProductsInBatchCommand
        {
            Items = [ new StockProductItemCommand { Id = productA.Id, QuantityUsed = 5 } ]
        };

        _productRepositoryMock.Setup(r => r.GetProductsByIds(
                It.IsAny<List<ProductId>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        _productRepositoryMock.Setup(r => r.SaveChanges(
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateConcurrencyException());

        // Act & Assert
        await Assert.ThrowsAsync<StockConcurrencyException>(() =>
            _handler.Handle(command, CancellationToken.None));
            
        _productRepositoryMock.Verify(r => r.SaveChanges(
            It.IsAny<CancellationToken>()), Times.Once);
    }
}