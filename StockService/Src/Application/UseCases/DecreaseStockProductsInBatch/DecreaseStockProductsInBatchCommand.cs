using MediatR;
using StockService.Application.Common.Dtos;

namespace StockService.Application.UseCases.DecreaseStockProductsInBatch;

public class DecreaseStockProductsInBatchCommand : IRequest<ApiResultDto<bool>>
{
    public List<StockProductItemCommand> Items { get; init; } = [];
    public Guid InvoiceNoteId { get; init; }
}