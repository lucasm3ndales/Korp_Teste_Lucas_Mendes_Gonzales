using System.ComponentModel.DataAnnotations;
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
    
    public uint Xmin { get; private set; }
    
    private readonly List<InvoiceNoteItem> _items = [];
    
    public IReadOnlyCollection<InvoiceNoteItem> Items => _items.AsReadOnly();
    
    public InvoiceNote()
    {
        Id = InvoiceNoteId.NewId();
        Status = InvoiceNoteStatus.OPEN; 
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
    
    public void AddItem(Guid productId, string code, string description, int quantity)
    {
        if (Status != InvoiceNoteStatus.OPEN)
            throw new InvalidInvoiceNoteStatusException(Status);
        

        var item = new InvoiceNoteItem(productId, code, description, quantity);
        
        _items.Add(item);
    }
    
    public void Close()
    {
        if (Status != InvoiceNoteStatus.PROCESSING)
            throw new InvalidInvoiceNoteStatusException(Status);
        

        if (_items.Count == 0)
            throw new InvoiceNoteEmptyException();
        

        Status = InvoiceNoteStatus.CLOSED;
    }

    public void Process()
    {
        if (Status == InvoiceNoteStatus.PROCESSING)
            return;
        
        if (Status != InvoiceNoteStatus.OPEN)
            throw new InvalidInvoiceNoteStatusException(Status);
        
        Status = InvoiceNoteStatus.PROCESSING;
    }
    
    public void RevertToOpen()
    {
        if (Status != InvoiceNoteStatus.PROCESSING)
            throw new InvalidInvoiceNoteStatusException(Status);
        
        Status = InvoiceNoteStatus.OPEN;
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