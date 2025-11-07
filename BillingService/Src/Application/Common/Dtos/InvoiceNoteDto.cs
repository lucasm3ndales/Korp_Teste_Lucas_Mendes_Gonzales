namespace BillingService.Application.Common;

public record InvoiceNoteDto(
    Guid Id,
    long NoteNumber,
    string Status,
    DateTimeOffset CreatedAt,
    IEnumerable<InvoiceNoteItemDto> Items
);