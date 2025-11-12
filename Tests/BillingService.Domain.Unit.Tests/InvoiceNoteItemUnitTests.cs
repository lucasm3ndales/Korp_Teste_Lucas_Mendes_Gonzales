using BillingService.Domain.Entities;
using BillingService.Domain.Exceptions;

namespace BillingService.Domain.Unit.Tests;

public class InvoiceNoteItemUnitTests
{
    [Fact(DisplayName = "Deve criar um item de nota fiscal quando os dados são válidos")]
    public void Should_CreateInvoiceNoteItem_When_DataIsValid()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var code = "PRD-001";
        var description = "Café Expresso";
        var quantity = 5;
        
        // Act
        var item = new InvoiceNoteItem(productId, code, description, quantity);

        // Assert
        Assert.NotNull(item);
        Assert.Equal(productId, item.ProductId);
        Assert.Equal(code, item.ProductCode);
        Assert.Equal(description, item.ProductDescription);
        Assert.Equal(quantity, item.Quantity);
    }
    
    [Theory(DisplayName = "Deve lançar InvalidInvoiceNoteItemQuantityException quando a quantidade for inválida")]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Should_Throw_InvalidInvoiceNoteItemQuantityException_When_QuantityIsInvalid(int invalidQuantity)
    {
        // Arrange
        var productId = Guid.NewGuid();
        var code = "PRD-001";
        var description = "Café Expresso";
        
        // Act & Assert
        Assert.Throws<InvalidInvoiceNoteItemQuantityException>(() => 
            new InvoiceNoteItem(productId, code, description, invalidQuantity)
        );
    }
    
    [Fact(DisplayName = "Deve lançar InvalidInvoiceNoteItemProductException quando o ProductId for Guid.Empty")]
    public void Should_Throw_InvalidInvoiceNoteItemProductException_When_ProductIdIsEmpty()
    {
        // Arrange
        var productId = Guid.Empty;
        var code = "PRD-001";
        var description = "Café Expresso";
        var quantity = 5;
        
        // Act & Assert
        Assert.Throws<InvalidInvoiceNoteItemProductException>(() => 
            new InvoiceNoteItem(productId, code, description, quantity)
        );
    }
    
    [Theory(DisplayName = "Deve lançar InvalidInvoiceNoteItemProductException quando o ProductCode for nulo ou vazio")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_Throw_InvalidInvoiceNoteItemProductException_When_ProductCodeIsNullOrEmpty(string? invalidCode)
    {
        // Arrange
        var productId = Guid.NewGuid();
        var description = "Café Expresso";
        var quantity = 5;
        
        // Act & Assert
        Assert.Throws<InvalidInvoiceNoteItemProductException>(() => 
            new InvoiceNoteItem(productId, invalidCode!, description, quantity)
        );
    }
    
    [Theory(DisplayName = "Deve lançar InvalidInvoiceNoteItemProductException quando a ProductDescription for nula ou vazia")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_Throw_InvalidInvoiceNoteItemProductException_When_ProductDescriptionIsNullOrEmpty(string? invalidDescription)
    {
        // Arrange
        var productId = Guid.NewGuid();
        var code = "PRD-001";
        var quantity = 5;
        
        // Act & Assert
        Assert.Throws<InvalidInvoiceNoteItemProductException>(() => 
            new InvoiceNoteItem(productId, code, invalidDescription!, quantity)
        );
    }
}