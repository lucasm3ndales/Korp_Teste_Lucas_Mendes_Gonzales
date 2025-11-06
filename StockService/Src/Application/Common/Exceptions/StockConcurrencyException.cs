namespace StockService.Application.Common.Exceptions;

public class StockConcurrencyException : StockApplicationException
{
    public StockConcurrencyException() 
        : base("Falha ao atualizar o estoque. Outro processo alterou este produto. " +
               "Por favor, tente novamente mais tarde.") { }
}