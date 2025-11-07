using BillingService.Domain.Exceptions;

namespace BillingService.Domain.ValueObjects;

public record InvoiceNoteId
{
    public Guid Value { get; }

    private InvoiceNoteId(Guid value)
    {
        if (value == Guid.Empty)
            throw new InvalidInvoiceNoteIdException();
        
        Value = value;
    }
    
    public static InvoiceNoteId NewId() => new(Guid.NewGuid());
    
    public static implicit operator Guid(InvoiceNoteId id) => id.Value;
    public static implicit operator InvoiceNoteId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
};