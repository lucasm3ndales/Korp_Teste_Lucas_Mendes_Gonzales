using Mapster;
using MediatR;
using StockService.Application.Common.Dtos;
using StockService.Application.Common.Exceptions;
using StockService.Application.Repositories;

namespace StockService.Application.UseCases.GetProductByProductId;

public class GetProductByProductIdQueryHandler(
    IProductRepository productRepository
) : IRequestHandler<GetProductByProductIdQuery, ApiResultDto<ProductDto>>
{
    public async Task<ApiResultDto<ProductDto>> Handle(GetProductByProductIdQuery request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByProductIdNoTracked(request.ProductId, cancellationToken);

        if (product == null)
            throw new ProductNotExistsException();
        
        return ApiResultDto<ProductDto>.Success(product.Adapt<ProductDto>());
    }
}