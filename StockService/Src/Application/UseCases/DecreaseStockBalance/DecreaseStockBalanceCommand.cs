using MediatR;
using StockService.Application.Common.Dtos;
using StockService.Domain.ValueObjects;

namespace StockService.Application.UseCases.DecreaseStockBalance;

public class DecreaseStockBalanceCommand : IRequest<ApiResultDto<bool>>
{
    public ProductId Id { get; set; }
    public int QuantityUsed { get; init; }
}