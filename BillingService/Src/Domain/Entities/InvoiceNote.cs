using BillingService.Domain.Enums;
using BillingService.Domain.Exceptions;
using BillingService.Domain.ValueObjects;

namespace BillingService.Domain.Entities;

public class InvoiceNote
{
    public InvoiceNoteId Id { get; private set; }
    
    public long NoteNumber { get; private set; }

    public InvoiceNoteStatus Status { get; private set; }
    
    public DateTimeOffset CreatedAt { get; private set; }
    
    public DateTimeOffset UpdatedAt { get; private set; }
    
    public byte[] RowVersion { get; private set; } = null!;

    private readonly List<InvoiceNoteItem> _items = [];
    
    public IReadOnlyCollection<InvoiceNoteItem> Items => _items.AsReadOnly();

    protected InvoiceNote() { }
    
    public InvoiceNote(long noteNumber)
    {
        Id = InvoiceNoteId.NewId();
        NoteNumber = noteNumber;
        Status = InvoiceNoteStatus.OPEN; 
        CreatedAt = DateTimeOffset.UtcNow;
    }
    
    public void AddItem(Guid productId, string code, string description, int quantity)
    {
        if (Status == InvoiceNoteStatus.CLOSED)
            throw new InvoiceNoteClosedException();
        

        var item = new InvoiceNoteItem(productId, code, description, quantity);
        
        _items.Add(item);
    }
    
    public void Close()
    {
        if (Status != InvoiceNoteStatus.OPEN)
            throw new InvoiceNoteClosedException();
        

        if (_items.Count == 0)
            throw new InvoiceNoteEmptyException();
        

        Status = InvoiceNoteStatus.CLOSED;
    }
    
    public void SetModified()
    {
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    private bool Equals(InvoiceNote? other)
    {
        if (other is null) return false;
        
        if (ReferenceEquals(this, other)) return true;
        
        return Id.Equals(other.Id);
    }
    
    public override bool Equals(object? obj) => Equals(obj as InvoiceNote);
    
    public override int GetHashCode() => Id.GetHashCode();
    
    public static bool operator ==(InvoiceNote? left, InvoiceNote? right)
    {
        if (left is null)
            return right is null;
        
        return left.Equals(right);
    }

    public static bool operator !=(InvoiceNote? left, InvoiceNote? right)
    {
        return !(left == right);
    }
}