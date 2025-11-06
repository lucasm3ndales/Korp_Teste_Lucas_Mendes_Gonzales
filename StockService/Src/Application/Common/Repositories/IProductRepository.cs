using StockService.Domain.Entities;
using StockService.Domain.ValueObjects;

namespace StockService.Application.Repositories;

public interface IProductRepository
{
    Task Add(Product product, CancellationToken cancellationToken);
    
    Task<Product?> GetById(ProductId id, CancellationToken cancellationToken);

    Task<Product?> GetByCode(string code, CancellationToken cancellationToken);
    
    Task SaveChanges(CancellationToken cancellationToken);
    
    Task<Product?> GetByProductIdNoTracked(ProductId id, CancellationToken cancellationToken);
    
    Task<IEnumerable<Product>?> GetAllProducts(CancellationToken cancellationToken);
    
    Task<IEnumerable<Product>?> GetAllProductsNoTracked(CancellationToken cancellationToken);
}