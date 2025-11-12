using BillingService.Domain.Entities;
using BillingService.Domain.ValueObjects;

namespace BillingService.Application.Common.Repositories;

public interface IInvoiceNoteRepository
{
    Task Add(InvoiceNote invoiceNote, CancellationToken cancellationToken);
    
    Task<InvoiceNote?> GetByIdWithItems(InvoiceNoteId id, CancellationToken cancellationToken);
    
    Task<long> GetLastNumberNote(CancellationToken cancellationToken);
    
    Task<InvoiceNote?> GetById(InvoiceNoteId id, CancellationToken cancellationToken);
    
    Task<IEnumerable<InvoiceNote>> ListAll(CancellationToken cancellationToken);
    
    Task SaveChanges(CancellationToken cancellationToken);

    Task<InvoiceNote?> GetByIdNoTracked(InvoiceNoteId id, CancellationToken cancellationToken);

    Task<IEnumerable<InvoiceNote>> ListAllNoTracked(CancellationToken cancellationToken);
    
    Task<IEnumerable<InvoiceNote>> GetProcessingInvoiceNotesOlderThan(
        DateTimeOffset date, 
        CancellationToken cancellationToken);
}