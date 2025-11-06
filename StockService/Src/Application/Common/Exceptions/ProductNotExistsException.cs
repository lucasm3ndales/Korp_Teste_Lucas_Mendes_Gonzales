namespace StockService.Application.Common.Exceptions;

public class ProductNotExistsException : StockApplicationException
{
    public ProductNotExistsException()
        : base("Produto não existente.")
    {
    }
}