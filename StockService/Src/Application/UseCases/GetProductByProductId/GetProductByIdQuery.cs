using MediatR;
using StockService.Application.Common.Dtos;
using StockService.Domain.ValueObjects;

namespace StockService.Application.UseCases.GetProductByProductId;

public class GetProductByIdQuery(ProductId id) : IRequest<ApiResultDto<ProductDto>>
{
    public ProductId Id { get; init; } = id;
}