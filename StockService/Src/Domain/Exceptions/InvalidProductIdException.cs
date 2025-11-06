namespace StockService.Domain.Exceptions;

public class InvalidProductIdException: StockDomainException
{
    public InvalidProductIdException() 
        : base("O Id do Produto não pode ser vazio.") { }
}