using Mapster;
using MediatR;
using StockService.Application.Common.Dtos;
using StockService.Application.Common.Exceptions;
using StockService.Application.Common.Repositories;
using StockService.Domain.ValueObjects;

namespace StockService.Application.UseCases.GetProductsByIds;

public class GetProductsByIdsQueryHandler(
    IProductRepository productRepository
) : IRequestHandler<GetProductsByIdsQuery, ApiResultDto<IEnumerable<ProductDto>>>
{
    public async Task<ApiResultDto<IEnumerable<ProductDto>>> Handle(GetProductsByIdsQuery request,
        CancellationToken cancellationToken)
    {
        if (request.ProductIds == null || !request.ProductIds.Any())
            throw new ProductIdsEmptyException();

        var productIds = request
            .ProductIds
            .Select(ProductId.From)
            .ToList();

        var products = await productRepository
            .GetByIdsNoTracked(productIds, cancellationToken);

        if (products == null || !products.Any())
            throw new ProductsNotFoundException();

        return ApiResultDto<IEnumerable<ProductDto>>.Success(
            products.Select(p => p.Adapt<ProductDto>())
        );
    }
}