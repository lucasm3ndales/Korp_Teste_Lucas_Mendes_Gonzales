namespace BillingService.Application.Common.Exceptions;

public class DecreaseStockProductsInBatchException: BillingApplicationException
{
    public List<string> ErrorMessages { get; }
    
    public DecreaseStockProductsInBatchException(List<string> errorMessages)
        : base(string.Join("; ", errorMessages))
    {
        ErrorMessages = errorMessages;
    }
}