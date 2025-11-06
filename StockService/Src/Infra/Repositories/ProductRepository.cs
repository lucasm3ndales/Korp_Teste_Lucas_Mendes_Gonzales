using Microsoft.EntityFrameworkCore;
using StockService.Application.Repositories;
using StockService.Domain.Entities;
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

    public async Task<Product?> GetByProductId(Guid productId, CancellationToken cancellationToken)
    {
        return await dbContext
            .Products.FirstOrDefaultAsync(
                p => p.ProductId == productId,
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

    public async Task<Product?> GetByProductIdNoTracked(Guid productId, CancellationToken cancellationToken)
    {
        return await dbContext.Products
            .AsNoTracking()
            .Where(p => p.ProductId == productId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>?> GetAllProducts(CancellationToken cancellationToken)
    {
        return await dbContext
            .Products
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>?> GetAllProductsNoTracked(CancellationToken cancellationToken)
    {
        return await dbContext
            .Products
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}