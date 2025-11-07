namespace BillingService.Application.Common;

public record InvoiceNoteItemDto(
    long Id,
    Guid ProductId,
    string ProductCode,
    string ProductDescription,
    int Quantity
    );