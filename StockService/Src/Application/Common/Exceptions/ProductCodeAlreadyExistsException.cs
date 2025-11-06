namespace StockService.Application.Common.Exceptions;

public class ProductCodeAlreadyExistsException: StockApplicationException
{
    public ProductCodeAlreadyExistsException(string code) 
        : base($"Um produto com o código '{code}' já existe no sistema.") { }
}