using StockService.Domain.ValueObjects;

namespace StockService.Application.UseCases.DecreaseStockProductsInBatch;

public class StockProductItemCommand()
{
    public ProductId Id { get; init; }
    public int QuantityUsed { get; init; }
}