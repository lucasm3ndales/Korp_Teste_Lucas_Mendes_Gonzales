using MediatR;
using Microsoft.EntityFrameworkCore;
using StockService.Application.Common.Dtos;
using StockService.Application.Common.Exceptions;
using StockService.Application.Repositories;
using StockService.Domain.Exceptions;

namespace StockService.Application.UseCases.DecreaseStockBalance;

public class DecreaseStockProductsInBatchCommandHandler(
    IProductRepository productRepository
) : IRequestHandler<DecreaseStockProductsInBatchCommand, ApiResultDto<bool>>
{
    public async Task<ApiResultDto<bool>> Handle(DecreaseStockProductsInBatchCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var productIds = request.Items.Select(item => item.Id).ToList();
            
            var products = await productRepository.GetProductsByIds(productIds, cancellationToken);

            var productDic = products.ToDictionary(p => p.Id);

            foreach (var i in request.Items)
            {
                if (!productDic.TryGetValue(i.Id, out var p))
                    throw new ProductNotExistsException();
                
                if (p.StockBalance < i.QuantityUsed)
                    throw new InsufficientStockBalanceException(p.Code, p.StockBalance,  i.QuantityUsed);
            }

            foreach (var i in request.Items)
            {
                var p = productDic[i.Id];
                p.DecreaseStockBalance(i.QuantityUsed);
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