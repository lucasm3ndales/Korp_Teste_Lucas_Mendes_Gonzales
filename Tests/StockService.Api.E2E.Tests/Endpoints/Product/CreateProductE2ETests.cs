using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using StockService.Application.Common.Dtos;
using StockService.Application.UseCases.CreateProduct;
using StockService.Infra.Data;

namespace StockService.Api.E2E.Tests.Endpoints;

public class CreateProductE2ETests: IClassFixture<CustomWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebAppFactory _factory;

    public CreateProductE2ETests(CustomWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        
        var dbContext = scope.ServiceProvider.GetRequiredService<StockDbContext>();
        dbContext.Database.EnsureCreated();
    }
    
    [Fact(DisplayName = "Deve retornar 201 Created quando o produto é válido")]
    public async Task Should_ReturnCreated_When_ProductIsValid()
    {
        // Arrange
        var code = "TRF-123";
        var command = new CreateProductCommand(code, "Chocolate", 10);

        // Act
        var response = await _client.PostAsJsonAsync("/api/products", command);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResultDto<ProductDto>>();
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(code, result.Data.Code);
    }

    [Fact(DisplayName = "Deve retornar 400 Bad Request quando o saldo inicial for negativo")]
    public async Task Should_ReturnBadRequest_When_InitialStockIsNegative()
    {
        // Arrange
        var command = new CreateProductCommand("TRF-123", "Chocolate", 10);

        // Act
        var response = await _client.PostAsJsonAsync("/api/products", command);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<ApiResultDto<object>>();
        Assert.False(result.IsSuccess);
        Assert.Contains("O Saldo (StockBalance) inicial '-5' é inválido", result.Messages.First());
    }
    
    [Fact(DisplayName = "Deve retornar 409 Conflict quando o Código já existir")]
    public async Task Should_ReturnConflict_When_CodeIsDuplicate()
    {
        // Arrange
        var command = new CreateProductCommand("TRF-123", "Chocolate", 10);
        await _client.PostAsJsonAsync("/api/products", command);

        // Act
        var response = await _client.PostAsJsonAsync("/api/products", command); 

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResultDto<object>>();
        Assert.False(result.IsSuccess);
        Assert.Contains("O 'Code' 'TRF-123' já está em uso", result.Messages.First());
    }
}