using Grpc.Core;
using MediatR;
using StockManager.Grpc;
using StockService.Application.UseCases.DecreaseStockBalance;
using StockService.Domain.Exceptions;

namespace StockService.Presentation.Grpc;

public class StockManagerService(
    IMediator mediator
) : StockManager.Grpc.StockManager.StockManagerBase
{
    public override async Task<DecreaseStockProductsInBatchResponse> DecreaseStockProductsInBatch(
        DecreaseStockProductsInBatchRequest request,
        ServerCallContext context)
    {
        var command = new DecreaseStockProductsInBatchCommand
        {
            Items = request.Items.Select(item => 
            {
                if (!Guid.TryParse(item.ProductId, out var id))
                    throw new InvalidProductIdException();

                return new StockProductItemCommand
                {
                    Id = id,
                    QuantityUsed = item.Quantity
                };
            }).ToList()
        };
        
        var result = await mediator.Send(command, context.CancellationToken);

        var response = new DecreaseStockProductsInBatchResponse
        {
            IsSuccess = result.IsSuccess,
            Data = result.Data
        };
        
        response.Messages.AddRange(result.Messages);
        
        return response;
    }
}