namespace BillingService.Application.Common.Exceptions;

public class InvoiceNoteItemsEmptyException: BillingApplicationException
{
    public InvoiceNoteItemsEmptyException() : base("A nota fiscal deve ter ao menos um produto.")
    {
    }
}