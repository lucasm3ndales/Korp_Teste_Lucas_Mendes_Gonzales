using Mapster;
using MediatR;
using StockService.Application.Common.Dtos;
using StockService.Application.Common.Exceptions;
using StockService.Application.Repositories;

namespace StockService.Application.UseCases.GetProductByProductId;

public class GetProductByIdQueryHandlerUnit(
    IProductRepository productRepository
) : IRequestHandler<GetProductByIdQuery, ApiResultDto<ProductDto>>
{
    public async Task<ApiResultDto<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByProductIdNoTracked(request.Id, cancellationToken);

        if (product == null)
            throw new ProductNotExistsException();
        
        return ApiResultDto<ProductDto>.Success(product.Adapt<ProductDto>());
    }
}