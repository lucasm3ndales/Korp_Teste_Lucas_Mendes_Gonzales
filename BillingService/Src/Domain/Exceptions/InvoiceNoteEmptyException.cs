namespace BillingService.Domain.Exceptions;

public class InvoiceNoteEmptyException : BillingDomainException
{
    public InvoiceNoteEmptyException() 
        : base("A operação não pode ser realizada pois a Nota Fiscal não possui itens.") { }
}