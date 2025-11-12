using StockService.Domain.Entities;

namespace StockService.Application.Common.Repositories;

public interface IIdempotencyKeyRepository
{
    Task Add(IdempotencyKey idempotencyKey, CancellationToken cancellationToken);

    Task SaveChanges(CancellationToken cancellationToken);

    Task<bool> Exists(Guid key, CancellationToken cancellationToken);
}