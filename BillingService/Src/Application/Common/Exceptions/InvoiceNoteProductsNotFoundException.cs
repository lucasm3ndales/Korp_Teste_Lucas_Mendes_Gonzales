namespace BillingService.Application.Common.Exceptions;

public class InvoiceNoteProductsNotFoundException: BillingApplicationException
{
    public List<string> ErrorMessages { get; }

    public InvoiceNoteProductsNotFoundException(List<string> errorMessages)
        : base(string.Join("; ", errorMessages))
    {
        ErrorMessages = errorMessages;
    }
    
    
    public InvoiceNoteProductsNotFoundException()
        : base("Um ou mais produtos não foram encontrados no serviço de estoque.")
    {
        ErrorMessages = new List<string> { "Um ou mais produtos não foram encontrados no serviço de estoque." };
    }
}