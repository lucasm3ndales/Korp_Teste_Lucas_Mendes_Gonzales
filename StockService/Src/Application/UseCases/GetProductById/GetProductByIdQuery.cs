using MediatR;
using StockService.Application.Common.Dtos;
using StockService.Domain.ValueObjects;

namespace StockService.Application.UseCases.GetProductById;

public class GetProductByIdQuery(ProductId id) : IRequest<ApiResultDto<ProductDto>>
{
    public ProductId Id { get; init; } = id;
}