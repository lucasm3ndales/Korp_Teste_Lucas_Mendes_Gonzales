using StockService.Domain.Entities;
using StockService.Domain.ValueObjects;

namespace StockService.Application.Common.Repositories;

public interface IProductRepository
{
    Task Add(Product product, CancellationToken cancellationToken);
    
    Task<Product?> GetById(ProductId id, CancellationToken cancellationToken);

    Task<Product?> GetByCode(string code, CancellationToken cancellationToken);
    
    Task SaveChanges(CancellationToken cancellationToken);
    
    Task<Product?> GetByIdNoTracked(ProductId id, CancellationToken cancellationToken);
    
    Task<IEnumerable<Product>?> ListAll(CancellationToken cancellationToken);
    
    Task<IEnumerable<Product>?> ListAllNoTracked(CancellationToken cancellationToken);
    
    Task<IEnumerable<Product>?> GetByIds(List<ProductId> ids, CancellationToken cancellationToken);
    
    Task<IEnumerable<Product>?> GetByIdsNoTracked(List<ProductId> ids, CancellationToken cancellationToken);
}