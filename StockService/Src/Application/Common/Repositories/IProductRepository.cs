using StockService.Domain.Entities;

namespace StockService.Application.Repositories;

public interface IProductRepository
{
    Task Add(Product product, CancellationToken cancellationToken);
    
    Task<Product?> GetByProductId(Guid productId, CancellationToken cancellationToken);

    Task<Product?> GetByCode(string code, CancellationToken cancellationToken);
    
    Task SaveChanges(CancellationToken cancellationToken);
    
    Task<Product?> GetByProductIdNoTracked(Guid productId, CancellationToken cancellationToken);
    
    Task<IEnumerable<Product>?> GetAllProducts(CancellationToken cancellationToken);
    
    Task<IEnumerable<Product>?> GetAllProductsNoTracked(CancellationToken cancellationToken);
}