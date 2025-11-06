namespace StockService.Domain.Exceptions;

public class InvalidStockBalanceQuantityException: StockDomainException
{
    public InvalidStockBalanceQuantityException(int quantity, string message) 
        : base($"Quantidade inválida: {quantity}. {message}") { }
}