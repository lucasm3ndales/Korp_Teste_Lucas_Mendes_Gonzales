namespace BillingService.Domain.Exceptions;

public class InvoiceNoteClosedException : BillingDomainException
{
    public InvoiceNoteClosedException() 
        : base("A operação não pode ser realizada pois a Nota Fiscal já está Fechada.") { }
}