namespace BillingService.Application.UseCases.CreateInvoiceNote;

public class CreateInvoiceNoteItemCommand
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}