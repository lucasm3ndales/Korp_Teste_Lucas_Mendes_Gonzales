using Microsoft.EntityFrameworkCore;
using StockService.Application.Common.Repositories;
using StockService.Domain.Entities;
using StockService.Domain.ValueObjects;
using StockService.Infra.Data;

namespace StockService.Infra.Repositories;

public class ProductRepository(
    StockDbContext dbContext
) : IProductRepository
{
    public async Task Add(Product product, CancellationToken cancellationToken)
    {
        await dbContext.Products.AddAsync(product, cancellationToken);
    }

    public async Task<Product?> GetById(ProductId id, CancellationToken cancellationToken)
    {
        return await dbContext
            .Products.FirstOrDefaultAsync(
                p => p.Id == id,
                cancellationToken
            );
    }

    public async Task<Product?> GetByCode(string code, CancellationToken cancellationToken)
    {
        var normalizedCode = code.ToUpper();
        return await dbContext
            .Products.FirstOrDefaultAsync(
                p => p.Code.ToUpper() == normalizedCode,
                cancellationToken
            );
    }

    public async Task SaveChanges(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Product?> GetByIdNoTracked(ProductId id, CancellationToken cancellationToken)
    {
        return await dbContext.Products
            .AsNoTracking()
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>?> ListAll(CancellationToken cancellationToken)
    {
        return await dbContext
            .Products
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>?> ListAllNoTracked(CancellationToken cancellationToken)
    {
        return await dbContext
            .Products
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>?> GetByIds(
        List<ProductId> ids, 
        CancellationToken cancellationToken)
    {
        return await dbContext
            .Products
            .Where(p => ids.Contains(p.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>?> GetByIdsNoTracked(List<ProductId> ids, CancellationToken cancellationToken)
    {
        return await dbContext
            .Products
            .AsNoTracking()
            .Where(p => ids.Contains(p.Id))
            .ToListAsync(cancellationToken);
    }
}