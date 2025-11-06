using MediatR;
using StockService.Application.Common.Dtos;

namespace StockService.Application.UseCases.GetProductByProductId;

public class GetProductByProductIdQuery(Guid productId): IRequest<ApiResultDto<ProductDto>>
{
    public Guid ProductId { get; private set; } = productId;
}