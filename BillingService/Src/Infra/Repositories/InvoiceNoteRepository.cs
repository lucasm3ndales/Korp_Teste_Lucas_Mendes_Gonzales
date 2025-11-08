using BillingService.Application.Common.Repositories;
using BillingService.Domain.Entities;
using BillingService.Domain.ValueObjects;
using BillingService.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace BillingService.Infra.Repositories;

public class InvoiceNoteRepository(
    BillingDbContext dbContext
    ): IInvoiceNoteRepository
{
    public async Task Add(InvoiceNote invoiceNote, CancellationToken cancellationToken)
    {
        await dbContext.InvoiceNotes.AddAsync(invoiceNote, cancellationToken);
    }

    public async Task<InvoiceNote?> GetByIdWithItems(InvoiceNoteId id, CancellationToken cancellationToken)
    {
        return await dbContext
            .InvoiceNotes
            .Include(i => i.Items)
            .Where(i => i.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<long> GetLastSequentialNumber(CancellationToken cancellationToken)
    {
        return await dbContext
            .InvoiceNotes
            .OrderBy(i => i.NoteNumber)
            .Select(i => i.NoteNumber)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<InvoiceNote?> GetById(InvoiceNoteId id, CancellationToken cancellationToken)
    {
        return await dbContext
            .InvoiceNotes
            .Where(i => i.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }
    
    public async Task<InvoiceNote?> GetByIdNoTracked(InvoiceNoteId id, CancellationToken cancellationToken)
    {
        return await dbContext
            .InvoiceNotes
            .AsNoTracking()
            .Where(i => i.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<InvoiceNote>> ListAll(CancellationToken cancellationToken)
    {
        return await dbContext
            .InvoiceNotes
            .OrderBy(i => i.NoteNumber)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<IEnumerable<InvoiceNote>> ListAllNoTracked(CancellationToken cancellationToken)
    {
        return await dbContext
            .InvoiceNotes
            .AsNoTracking()
            .OrderBy(i => i.NoteNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task SaveChanges(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}