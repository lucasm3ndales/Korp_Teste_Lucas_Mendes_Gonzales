using MediatR;
using Microsoft.EntityFrameworkCore;
using StockService.Application.Common.Dtos;
using StockService.Application.Common.Exceptions;
using StockService.Application.Common.Repositories;
using StockService.Domain.Exceptions;

namespace StockService.Application.UseCases.DecreaseStockProductsInBatch;

public class DecreaseStockProductsInBatchCommandHandler(
    IProductRepository productRepository
) : IRequestHandler<DecreaseStockProductsInBatchCommand, ApiResultDto<bool>>
{
   public async Task<ApiResultDto<bool>> Handle(DecreaseStockProductsInBatchCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var items = request
                .Items
                .GroupBy(i => i.Id)
                .Select(g => new 
                {
                    ProductId = g.Key,
                    TotalQuantityUsed = g.Sum(item => item.QuantityUsed)
                })
                .ToList();

            var productIds = items.Select(item => item.ProductId).ToList();
            
            var products = await productRepository.GetByIds(productIds, cancellationToken);
            
            var productDic = products.ToDictionary(p => p.Id);
            
            foreach (var i in items)
            {
                if (!productDic.TryGetValue(i.ProductId, out var p))
                    throw new ProductNotExistsException();
                
                if (p.StockBalance < i.TotalQuantityUsed)
                    throw new InsufficientStockBalanceException(p.Code, p.StockBalance,  i.TotalQuantityUsed);
            }
            
            foreach (var i in items)
            {
                var p = productDic[i.ProductId];
                p.DecreaseStockBalance(i.TotalQuantityUsed);
            }
            
            await productRepository.SaveChanges(cancellationToken);

            return ApiResultDto<bool>.Success("Estoque atualizado com sucesso para todos os itens!", true);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new StockConcurrencyException();
        }
    }
}