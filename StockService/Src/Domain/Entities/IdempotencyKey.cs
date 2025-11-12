namespace StockService.Domain.Entities;

public class IdempotencyKey(Guid key)
{
    public Guid Key { get; private set; } = key;
    public DateTimeOffset ProcessedAt { get; private set; } = DateTimeOffset.UtcNow;
}