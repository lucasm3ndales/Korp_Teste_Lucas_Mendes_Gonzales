namespace StockService.Application.Common.Exceptions;

public class ProductIdsEmptyException: StockApplicationException
{
    public ProductIdsEmptyException(): base("Ao menos um ID de produto deve ser enviado.") {}
}