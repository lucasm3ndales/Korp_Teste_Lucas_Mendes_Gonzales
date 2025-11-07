namespace BillingService.Domain.Exceptions;

public class BillingDomainException: Exception
{
    protected BillingDomainException(string message) : base(message) { }

}