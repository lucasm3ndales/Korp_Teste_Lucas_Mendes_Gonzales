using BillingService.Domain.Enums;

namespace BillingService.Domain.Exceptions;

public class InvalidInvoiceNoteStatusException : BillingDomainException
{
    public InvalidInvoiceNoteStatusException(InvoiceNoteStatus currentStatus) 
        : base($"A operação não é permitida pois a Nota Fiscal está com o status: '{currentStatus}'.") { }
}