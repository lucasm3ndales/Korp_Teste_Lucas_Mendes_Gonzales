namespace StockService.Application.Common.Exceptions;

public class ProductsNotFoundException: StockApplicationException
{
    public ProductsNotFoundException() : base("Nenhum produto foi encontrado.") { }
}