namespace BillingService.Application.Common;

public record InvoiceNoteDto(
    Guid Id,
    long NoteNumber,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    IEnumerable<InvoiceNoteItemDto> Items
);