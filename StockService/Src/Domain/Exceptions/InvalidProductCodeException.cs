namespace StockService.Domain.Exceptions;

public class InvalidProductCodeException: StockDomainException
{
    public InvalidProductCodeException(string code) 
        : base($"O Código '{code}' é inválido. Ele não pode ser nulo e deve ter no máximo 20 caracteres.") { }
}