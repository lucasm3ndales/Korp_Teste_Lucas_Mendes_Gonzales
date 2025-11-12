using Microsoft.EntityFrameworkCore;
using StockService.Application.Common.Repositories;
using StockService.Domain.Entities;
using StockService.Infra.Data;

namespace StockService.Infra.Repositories;

public class IdempotencyKeyRepository(StockDbContext dbContext) : IIdempotencyKeyRepository
{
    public async Task Add(IdempotencyKey idempotencyKey, CancellationToken cancellationToken)
    {
        await dbContext.IdempotencyKeys.AddAsync(idempotencyKey, cancellationToken);
    }

    public async Task SaveChanges(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> Exists(Guid key, CancellationToken cancellationToken)
    {
        return await dbContext
            .IdempotencyKeys
            .AsNoTracking()
            .FirstOrDefaultAsync(i =>
                    i.Key == key,
                cancellationToken: cancellationToken
            ) != null;
    }
}