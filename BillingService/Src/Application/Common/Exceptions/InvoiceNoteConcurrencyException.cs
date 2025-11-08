namespace BillingService.Application.Common.Exceptions;

public class InvoiceNoteConcurrencyException: BillingApplicationException
{
    public InvoiceNoteConcurrencyException()
        : base("Ocorreu um conflito de concorrência. A nota fiscal já está em processamento " +
               "ou foi modificada por outro processo. Tente novamente.")
    {
    }
}