namespace StockService.Domain.Exceptions;

public class InvalidProductDescriptionException: StockDomainException
{
    public InvalidProductDescriptionException() 
        : base("A Descrição não pode ser nula ou vazia.") { }
}