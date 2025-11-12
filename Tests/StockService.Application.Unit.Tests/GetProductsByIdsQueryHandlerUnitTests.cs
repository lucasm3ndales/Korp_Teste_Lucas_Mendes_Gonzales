using Moq;
using StockService.Application.Common.Exceptions;
using StockService.Application.Common.Repositories;
using StockService.Application.UseCases.GetProductsByIds;
using StockService.Domain.Entities;
using StockService.Domain.ValueObjects;

namespace StockService.Application.Unit.Tests;

public class GetProductsByIdsQueryHandlerUnitTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly GetProductsByIdsQueryHandler _handler;

    public GetProductsByIdsQueryHandlerUnitTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _handler = new GetProductsByIdsQueryHandler(_productRepositoryMock.Object);
    }

    [Fact(DisplayName = "Deve retornar sucesso com lista de produtos quando IDs são válidos e encontrados")]
    public async Task Should_ReturnSuccessWithProductList_When_IdsAreValidAndFound()
    {
        // Arrange
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var query = new GetProductsByIdsQuery { ProductIds = new List<Guid> { id1, id2 } };

        var productList = new List<Product>
        {
            new Product("HCL-123", "Café", 10),
            new Product("FEX-456", "Leite", 20)
        };

        productList[0]
            .GetType()
            .GetProperty(
                nameof(Product.Id))?.SetValue(productList[0],
                ProductId.From(id1)
            );

        productList[1]
            .GetType()
            .GetProperty(
                nameof(Product.Id))?.SetValue(productList[1],
                ProductId.From(id2)
            );

        _productRepositoryMock.Setup(r => r.GetByIdsNoTracked(
                It.IsAny<List<ProductId>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(productList);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count());
        Assert.Contains(result.Data, p => p.Id == id1);
        Assert.Contains(result.Data, p => p.Id == id2);
    }

    [Fact(DisplayName = "Deve lançar ProductsNotFoundException quando nenhum produto for encontrado")]
    public async Task Should_Throw_ProductsNotFoundException_When_NoProductsAreFound()
    {
        // Arrange
        var query = new GetProductsByIdsQuery { ProductIds = new List<Guid> { Guid.NewGuid() } };

        _productRepositoryMock.Setup(r => r.GetByIdsNoTracked(
                It.IsAny<List<ProductId>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product>());

        // Act & Assert
        await Assert.ThrowsAsync<ProductsNotFoundException>(() =>
            _handler.Handle(query, CancellationToken.None));
    }

    [Theory(DisplayName = "Deve lançar ProductIdsEmptyException quando a lista de IDs for nula ou vazia")]
    [InlineData(null)]
    [InlineData(0)]
    public async Task Should_Throw_ProductIdsEmptyException_When_ProductIdsIsNullOrEmpty(int? count)
    {
        // Arrange
        var productIds = count.HasValue ? new List<Guid>() : null;
        if (count.HasValue && count.Value > 0)
        {
            productIds.AddRange(Enumerable.Range(0, count.Value).Select(_ => Guid.NewGuid()));
        }

        var query = new GetProductsByIdsQuery { ProductIds = productIds! };

        // Act & Assert
        await Assert.ThrowsAsync<ProductIdsEmptyException>(() =>
            _handler.Handle(query, CancellationToken.None));

        _productRepositoryMock.Verify(r => r.GetByIdsNoTracked(
            It.IsAny<List<ProductId>>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }
}