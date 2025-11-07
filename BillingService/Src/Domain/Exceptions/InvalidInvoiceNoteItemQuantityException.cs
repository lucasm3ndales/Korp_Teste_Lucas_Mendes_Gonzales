namespace BillingService.Domain.Exceptions;

public class InvalidInvoiceNoteItemQuantityException: BillingDomainException
{
    public InvalidInvoiceNoteItemQuantityException() 
        : base("A quantidade de um item da nota deve ser maior que zero.") { }
}