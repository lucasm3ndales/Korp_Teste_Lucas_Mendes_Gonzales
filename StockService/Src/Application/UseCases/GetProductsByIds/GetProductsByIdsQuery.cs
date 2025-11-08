using MediatR;
using StockService.Application.Common.Dtos;

namespace StockService.Application.UseCases.GetProductsByIds;

public class GetProductsByIdsQuery: IRequest<ApiResultDto<IEnumerable<ProductDto>>>
{
    public List<Guid>  ProductIds { get; set; }
}