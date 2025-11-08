namespace BillingService.Application.Common.Exceptions;

public class InvoiceNoteNotExistsException: BillingApplicationException
{
    public InvoiceNoteNotExistsException() : base("Nota fiscal não existente.")
    {
    }
}