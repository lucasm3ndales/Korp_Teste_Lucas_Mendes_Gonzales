using MediatR;
using Microsoft.EntityFrameworkCore;
using StockService.Application.Common.Dtos;
using StockService.Application.Common.Exceptions;
using StockService.Application.Repositories;

namespace StockService.Application.UseCases.DecreaseStockBalance;

public class DecreaseStockBalanceCommandHandler(
    IProductRepository productRepository
) : IRequestHandler<DecreaseStockBalanceCommand, ApiResultDto<bool>>
{
    public async Task<ApiResultDto<bool>> Handle(DecreaseStockBalanceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await productRepository.GetByProductId(request.ProductId, cancellationToken);

            if (product == null)
                throw new ProductNotExistsException();
        
            product.DecreaseStockBalance(request.QuantityUsed);
            await productRepository.SaveChanges(cancellationToken);
        
            return ApiResultDto<bool>.Success("Estoque atualizado com sucesso!", true);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new StockConcurrencyException();
        }
    }
}
