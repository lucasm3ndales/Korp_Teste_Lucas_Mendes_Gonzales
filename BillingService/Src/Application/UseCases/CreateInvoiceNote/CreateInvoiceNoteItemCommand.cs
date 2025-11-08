namespace BillingService.Application.UseCases.CreateInvoiceNote;

public class CreateInvoiceNoteItemCommand
{
    public Guid ProductId { get; set; }
    public string ProductCode { get; set; }
    public string ProductDescription { get; set; }
    public int Quantity { get; set; }
}