using BillingService.Domain.Exceptions;
using BillingService.Domain.ValueObjects;

namespace BillingService.Domain.Entities;

public class InvoiceNoteItem
{
    public long Id { get; private set; }

    public InvoiceNoteId InvoiceNoteId { get; private set; }

    public Guid ProductId { get; private set; } 

    public string ProductCode { get; private set; }

    public string ProductDescription { get; private set; }

    public int Quantity { get; private set; }

    protected InvoiceNoteItem() { }

    public InvoiceNoteItem(
        Guid productId, 
        string productCode, 
        string productDescription, 
        int quantity)
    {
        if (quantity <= 0)
            throw new InvalidInvoiceNoteItemQuantityException();
        
        if (productId == Guid.Empty 
            || string.IsNullOrWhiteSpace(productCode) 
            || string.IsNullOrWhiteSpace(productDescription))
            throw new InvalidInvoiceNoteItemProductException();
        
        ProductId = productId;
        ProductCode = productCode;
        ProductDescription = productDescription;
        Quantity = quantity;
    }
}