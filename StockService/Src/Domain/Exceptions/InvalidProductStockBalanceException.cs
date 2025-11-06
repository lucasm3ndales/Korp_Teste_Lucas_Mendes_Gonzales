namespace StockService.Domain.Exceptions;

public class InvalidProductStockBalanceException: StockDomainException
{
    public InvalidProductStockBalanceException(int balance) 
        : base($"O Saldo (StockBalance) inicial '{balance}' é inválido. Ele não pode ser negativo.") { }
}