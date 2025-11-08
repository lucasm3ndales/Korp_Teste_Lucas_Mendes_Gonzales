using Mapster;
using MediatR;
using StockService.Application.Common.Dtos;
using StockService.Application.Repositories;

namespace StockService.Application.UseCases.GetAllProducts;

public class GetAllProductsQueryHandler(
    IProductRepository productRepository
) : IRequestHandler<GetAllProductsQuery, ApiResultDto<IEnumerable<ProductDto>>>
{
    public async Task<ApiResultDto<IEnumerable<ProductDto>>> Handle(GetAllProductsQuery request,
        CancellationToken cancellationToken)
    {
        var products = await productRepository
            .ListAllNoTracked(cancellationToken);

        if (!products.Any())
            return ApiResultDto<IEnumerable<ProductDto>>.Success([]);

        return ApiResultDto<IEnumerable<ProductDto>>.Success(products.Select(p => p.Adapt<ProductDto>()));
    }
}