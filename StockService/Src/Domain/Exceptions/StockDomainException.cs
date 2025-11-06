namespace StockService.Domain.Exceptions;

public class StockDomainException: Exception
{
    protected StockDomainException(string message) : base(message) { }
}