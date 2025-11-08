using MediatR;
using StockService.Application.Common.Dtos;
using StockService.Domain.ValueObjects;

namespace StockService.Application.UseCases.DecreaseStockBalance;

public class DecreaseStockProductsInBatchCommand : IRequest<ApiResultDto<bool>>
{
    public List<StockProductItemCommand> Items { get; init; } = [];
}