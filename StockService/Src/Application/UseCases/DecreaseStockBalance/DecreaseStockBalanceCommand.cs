using MediatR;
using StockService.Application.Common.Dtos;

namespace StockService.Application.UseCases.DecreaseStockBalance;

public class DecreaseStockBalanceCommand : IRequest<ApiResultDto<bool>>
{
    public Guid ProductId { get; init; }
    public int QuantityUsed{ get; init; }
}