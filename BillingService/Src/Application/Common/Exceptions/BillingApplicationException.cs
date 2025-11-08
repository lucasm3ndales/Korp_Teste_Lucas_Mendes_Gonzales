namespace BillingService.Application.Common.Exceptions;

public class BillingApplicationException: Exception
{
    protected BillingApplicationException(string message) : base(message) { }
}