namespace StockService.Domain.Exceptions;

public class InsufficientStockBalanceException: StockDomainException
{
    public InsufficientStockBalanceException(string code, int balance, int required) 
        : base($"Estoque insuficiente para o produto {code}. Saldo: {balance}, Requerido: {required}") { }
}