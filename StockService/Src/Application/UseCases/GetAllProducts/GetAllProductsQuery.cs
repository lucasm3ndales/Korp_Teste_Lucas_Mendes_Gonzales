using MediatR;
using StockService.Application.Common.Dtos;

namespace StockService.Application.UseCases.GetAllProducts;

public class GetAllProductsQuery: IRequest<ApiResultDto<IEnumerable<ProductDto>>>
{
    
}