namespace BillingService.Application.Common.Exceptions;

public class InvoiceNoteItemsEmptyException: BillingApplicationException
{
    protected InvoiceNoteItemsEmptyException() : base("A nota fiscal deve ter ao menos um produto.")
    {
    }
}