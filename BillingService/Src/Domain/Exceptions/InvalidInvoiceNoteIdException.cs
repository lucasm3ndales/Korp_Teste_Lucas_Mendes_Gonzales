namespace BillingService.Domain.Exceptions;

public class InvalidInvoiceNoteIdException: BillingDomainException
{
    public InvalidInvoiceNoteIdException() 
        : base("O Id da nota fiscal não pode ser vazio.") { }
}