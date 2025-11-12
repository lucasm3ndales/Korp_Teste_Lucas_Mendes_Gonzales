using Microsoft.EntityFrameworkCore;
using Moq;
using StockService.Application.Common.Exceptions;
using StockService.Application.Common.Repositories;
using StockService.Application.Common.Services.TransactionManager;
using StockService.Application.UseCases.DecreaseStockProductsInBatch;
using StockService.Domain.Entities;
using StockService.Domain.Exceptions;
using StockService.Domain.ValueObjects;

namespace StockService.Application.Unit.Tests;

public class DecreaseStockProductsInBatchCommandHandlerUnitTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IIdempotencyKeyRepository> _idempotencyKeyRepositoryMock;
    private readonly Mock<ITransactionManagerService> _transactionManagerServiceMock;
    private readonly DecreaseStockProductsInBatchCommandHandler _handler;

    public DecreaseStockProductsInBatchCommandHandlerUnitTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _idempotencyKeyRepositoryMock = new Mock<IIdempotencyKeyRepository>();
        _transactionManagerServiceMock = new Mock<ITransactionManagerService>();
        
        _transactionManagerServiceMock
            .Setup(t => t.BeginTransaction(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Mock.Of<IDisposable>());
        
        _handler = new DecreaseStockProductsInBatchCommandHandler(
            _productRepositoryMock.Object,
            _idempotencyKeyRepositoryMock.Object,
            _transactionManagerServiceMock.Object
            );
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

        _productRepositoryMock.Setup(r => r.GetByIds(
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
        var notFoundProductId = ProductId.NewId();
        
        var command = new DecreaseStockProductsInBatchCommand
        {
            Items =
            [
                new StockProductItemCommand { Id = productA.Id, QuantityUsed = 5 },
                new StockProductItemCommand { Id = notFoundProductId, QuantityUsed = 5 }
            ]
        };

        _productRepositoryMock.Setup(r => r.GetByIds(
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
        var productA = new Product("HCL-123", "Café", 10);
        var productB = new Product("HCL-456", "Açúcar", 5);
        var products = new List<Product> { productA, productB };

        var command = new DecreaseStockProductsInBatchCommand
        {
            Items =
            [
                new StockProductItemCommand { Id = productA.Id, QuantityUsed = 5 },
                new StockProductItemCommand { Id = productB.Id, QuantityUsed = 10 }
            ]
        };

        _productRepositoryMock.Setup(r => r.GetByIds(
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

        _productRepositoryMock.Setup(r => r.GetByIds(
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
    
    [Fact(DisplayName = "Deve retornar sucesso e não processar quando a chave de idempotência existir")]
    public async Task Should_ReturnSuccessAndNotProcess_When_IdempotencyKeyExists()
    {
        // Arrange
        var invoiceNoteId = Guid.NewGuid();
        var command = new DecreaseStockProductsInBatchCommand { InvoiceNoteId = invoiceNoteId, Items = [] };

        _idempotencyKeyRepositoryMock
            .Setup(r => r.Exists(invoiceNoteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        _productRepositoryMock.Verify(r => r.GetByIds(
            It.IsAny<List<ProductId>>(), It.IsAny<CancellationToken>()), Times.Never);

        _transactionManagerServiceMock.Verify(t => t.CommitTransaction(
            It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact(DisplayName = "Deve lançar StockConcurrencyException quando o Xmin não coincidir")]
    public async Task Should_Throw_StockConcurrencyException_When_XminMismatch()
    {
        // Arrange
        var productA = new Product("HCL-123", "Café", 10);
        productA.GetType().GetProperty(nameof(Product.Xmin))?.SetValue(productA, (uint)2);

        var command = new DecreaseStockProductsInBatchCommand
        {
            Items = [ new StockProductItemCommand { Id = productA.Id, QuantityUsed = 5, Xmin = 1 } ]
        };
        var products = new List<Product> { productA };

        _productRepositoryMock.Setup(r => r.GetByIds(
                It.IsAny<List<ProductId>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        // Act & Assert
        await Assert.ThrowsAsync<StockConcurrencyException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _productRepositoryMock.Verify(r => r.SaveChanges(
            It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact(DisplayName = "Deve agrupar e somar quantidades de itens duplicados")]
    public async Task Should_GroupAndSumQuantities_When_ItemsAreDuplicated()
    {
        // Arrange
        var productA = new Product("HCL-123", "Café", 10);
        var products = new List<Product> { productA };

        var command = new DecreaseStockProductsInBatchCommand
        {
            Items =
            [
                new StockProductItemCommand { Id = productA.Id, QuantityUsed = 3 },
                new StockProductItemCommand { Id = productA.Id, QuantityUsed = 2 }
            ]
        };

        _productRepositoryMock.Setup(r => r.GetByIds(
                It.IsAny<List<ProductId>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(5, productA.StockBalance); 

        _productRepositoryMock.Verify(r => r.SaveChanges(
            It.IsAny<CancellationToken>()), Times.Once);
    }
}