using Mapster;
using Moq;
using StockService.Application.Common.Dtos;
using StockService.Application.Common.Exceptions;
using StockService.Application.Repositories;
using StockService.Application.UseCases.GetProductById;
using StockService.Domain.Entities;
using StockService.Domain.ValueObjects;

namespace StockService.Application.Unit.Tests;

public class GetProductByIdQueryHandlerUnitTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly GetProductByIdQueryHandler _handler;

    public GetProductByIdQueryHandlerUnitTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _handler = new GetProductByIdQueryHandler(_productRepositoryMock.Object);
        
        TypeAdapterConfig.GlobalSettings.Scan(typeof(GetProductByIdQueryHandler).Assembly);
        TypeAdapterConfig<Product, ProductDto>.NewConfig()
            .Map(dest => dest.Id, src => src.Id);
    }

    [Fact(DisplayName = "Deve retornar sucesso com o DTO do produto quando o produto existir")]
    public async Task Should_ReturnSuccessWithProduct_When_ProductExists()
    {
        // Arrange
        var productId = ProductId.NewId();
        var query = new GetProductByIdQuery(productId);
        
        // Criar um produto mockado
        var product = new Product("HCL-123", "Café", 10);
        
        _productRepositoryMock.Setup(r => r.GetByProductIdNoTracked(
                productId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(product.Id, result.Data.Id);
        Assert.Equal(product.Code, result.Data.Code);
    }

    [Fact(DisplayName = "Deve lançar ProductNotExistsException quando o produto não for encontrado")]
    public async Task Should_Throw_ProductNotExistsException_When_ProductIsNotFound()
    {
        // Arrange
        var productId = ProductId.NewId();
        var query = new GetProductByIdQuery(productId);

        _productRepositoryMock.Setup(r => r.GetByProductIdNoTracked(
                productId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product)null!);

        // Act & Assert
        await Assert.ThrowsAsync<ProductNotExistsException>(() => 
            _handler.Handle(query, CancellationToken.None));
    }
}