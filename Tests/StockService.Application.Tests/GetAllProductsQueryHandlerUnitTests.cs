using Mapster;
using Moq;
using StockService.Application.Common.Dtos;
using StockService.Application.Repositories;
using StockService.Application.UseCases.GetAllProducts;
using StockService.Domain.Entities;

namespace StockService.Application.Tests;

public class GetAllProductsQueryHandlerUnitTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly GetAllProductsQueryHandlerUnit _handlerUnit;

    public GetAllProductsQueryHandlerUnitTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _handlerUnit = new GetAllProductsQueryHandlerUnit(_productRepositoryMock.Object);

        TypeAdapterConfig.GlobalSettings.Scan(typeof(GetAllProductsQueryHandlerUnit).Assembly);
        TypeAdapterConfig<Product, ProductDto>.NewConfig()
            .Map(dest => dest.Id, src => src.Id);
    }

    [Fact]
    public async Task Should_ReturnSuccessWithProductList_When_ProductsExist()
    {
        // Arrange
        var query = new GetAllProductsQuery();
        var productList = new List<Product>
        {
            new("HCL-123", "Café", 10),
            new("FEX-123", "Leite", 20)
        };

        _productRepositoryMock.Setup(r => r.GetAllProductsNoTracked(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(productList);

        // Act
        var result = await _handlerUnit.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count());
        Assert.Equal("HCL-123", result.Data.First().Code);
    }

    [Fact]
    public async Task Should_ReturnSuccessWithEmptyList_When_NoProductsExist()
    {
        // Arrange
        var query = new GetAllProductsQuery();
        var emptyList = new List<Product>();

        _productRepositoryMock.Setup(r => r.GetAllProductsNoTracked(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyList);

        // Act
        var result = await _handlerUnit.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data);
    }
}