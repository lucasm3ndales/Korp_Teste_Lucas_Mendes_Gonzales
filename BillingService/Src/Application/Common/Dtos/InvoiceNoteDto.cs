namespace BillingService.Application.Common;

public record InvoiceNoteDto(
    Guid Id,
    long NoteNumber,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    uint Xmin,
    IEnumerable<InvoiceNoteItemDto> Items
);