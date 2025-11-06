namespace StockService.Application.Common.Exceptions;

public class StockApplicationException: Exception
{
    protected StockApplicationException(string message) : base(message) { }
}