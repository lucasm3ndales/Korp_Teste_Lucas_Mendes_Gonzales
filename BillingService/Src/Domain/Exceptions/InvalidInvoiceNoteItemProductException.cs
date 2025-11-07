namespace BillingService.Domain.Exceptions;

public class InvalidInvoiceNoteItemProductException: BillingDomainException
{
    public InvalidInvoiceNoteItemProductException() 
        : base("Dados do produto inválidos. O ID, Código e Descrição do produto não podem ser vazios.") { }
}