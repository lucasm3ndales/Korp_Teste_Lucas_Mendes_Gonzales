namespace BillingService.Application.Common.Exceptions;

public class DecreaseStockProductsInBatchException: BillingApplicationException
{
    public DecreaseStockProductsInBatchException(string message)
        : base(message)
    {
    }
}