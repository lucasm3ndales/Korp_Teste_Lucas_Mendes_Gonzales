using StockService.Domain.Entities;
using StockService.Domain.Exceptions;

namespace StockService.Domain.Unit.Tests;

public class ProductTests
{
    [Fact(DisplayName = "Deve criar um produto quando os dados são válidos")]
    public void Should_CreateProduct_When_DataIsValid()
    {
        // Arrange
        var code = "LXT-123";
        var description = "Café";
        var initialStockBalance = 10;
        
        // Act
        var product = new Product(code, description, initialStockBalance);

        // Assert
        Assert.NotNull(product);
        Assert.Equal(code, product.Code);
        Assert.Equal(description, product.Description);
        Assert.Equal(initialStockBalance, product.StockBalance);
    }
    
    [Theory(DisplayName = "Deve lançar exceção quando o Código for nulo ou vazio")]
    [InlineData(null)]
    [InlineData(" ")]
    [InlineData("")]
    public void Should_Throw_InvalidProductCodeException_When_CodeIsNullOrEmpty(string? code)
    {
        // Act and Assert
        Assert.Throws<InvalidProductCodeException>(() => new Product(code!, "Café", 10));
    }
    
    [Fact(DisplayName = "Deve lançar exceção quando o Código for muito longo")]
    public void Should_Throw_InvalidProductCodeException_When_CodeIsTooLong()
    {
        // Arrange
        var code = new string('a', 21);
        
        // Act and Assert
        Assert.Throws<InvalidProductCodeException>(() => new Product(code, "Café", 10));
    }
    
    [Theory(DisplayName = "Deve lançar exceção quando a Descrição for nula ou vazia")]
    [InlineData(null)]
    [InlineData(" ")]
    [InlineData("")]
    public void Should_Throw_InvalidProductDescriptionException_When_DescriptionIsNullOrEmpty(string? invalidDescription)
    {
        // Act & Assert
        Assert.Throws<InvalidProductDescriptionException>(() => 
            new Product("LXT-123", invalidDescription!, 10)
        );
    }

    [Fact(DisplayName = "Deve lançar exceção quando a Descrição for muito longa")]
    public void Should_Throw_InvalidProductDescriptionException_When_DescriptionIsTooLong()
    {
        // Arrange
        var description = new string('a', 256);
    
        // Act & Assert
        Assert.Throws<InvalidProductDescriptionException>(() => 
            new Product("LXT-123", description, 10)
        );
    }
    
    [Theory(DisplayName = "Deve lançar exceção quando o Saldo Inicial for negativo")]
    [InlineData(-1)]
    public void Should_Throw_InvalidProductStockBalanceException_When_InitialStockBalanceIsNegative(int initialStockBalance)
    {
        // Act and Assert
        Assert.Throws<InvalidProductStockBalanceException>(() => 
            new Product("LXT-123", "Café", initialStockBalance)
        );
    }
    
    [Fact(DisplayName = "Deve atualizar o saldo ao diminuir uma quantidade válida")]
    public void Should_UpdateStockBalance_When_DecreasingValidQuantity()
    {
        // Arrange
        var product = new Product("LXT-123", "Café", 10);
        
        // Act
        product.DecreaseStockBalance(3);
        
        // Assert
        Assert.Equal(7, product.StockBalance);
    }
    
    [Fact(DisplayName = "Deve lançar exceção quando diminuir mais que o saldo disponível")]
    public void Should_Throw_InsufficientStockBalanceException_When_DecreasingMoreThanAvailable()
    {
        // Arrange
        var product = new Product("LXT-123", "Café", 10);
        var quantityToUse = 11;
        
        // Act & Assert
        Assert.Throws<InsufficientStockBalanceException>(() => 
            product.DecreaseStockBalance(quantityToUse)
        );
    }
    
    [Fact(DisplayName = "Deve lançar exceção quando diminuir uma quantidade negativa ou zero")]
    public void Should_Throw_InvalidStockBalanceQuantityException_When_DecreasingNegativeQuantity()
    {
        // Arrange
        var product = new Product("LXT-123", "Café", 10);
        var invalidQuantity = -5;
        
        // Act & Assert
        Assert.Throws<InvalidStockBalanceQuantityException>(() => 
            product.DecreaseStockBalance(invalidQuantity)
        );
    }
}