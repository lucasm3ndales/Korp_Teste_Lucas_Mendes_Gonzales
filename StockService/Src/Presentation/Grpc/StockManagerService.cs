using Grpc.Core;
using Mapster;
using MediatR;
using StockManager.Grpc;
using StockService.Application.UseCases.DecreaseStockProductsInBatch;
using StockService.Application.UseCases.GetProductsByIds;
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
                    QuantityUsed = item.Quantity,
                    Xmin = item.Xmin,
                };
            }).ToList(),
            InvoiceNoteId = Guid.TryParse(request.InvoiceNoteId,  out var id) ? id : Guid.Empty
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

    public override async Task<GetProductsByIdsResponse> GetProductsByIds(GetProductsByIdsRequest request,
        ServerCallContext context)
    {
        var command = new GetProductsByIdsQuery
        {
            ProductIds = request.ProductIds.Select(i =>
            {
                if (!Guid.TryParse(i, out var id))
                    throw new InvalidProductIdException();

                return id;
            }).ToList()
        };

        var result = await mediator.Send(command, context.CancellationToken);

        var response = new GetProductsByIdsResponse
        {
            IsSuccess = result.IsSuccess,
        };

        response.Messages.AddRange(result.Messages);

        if (result.Data != null && result.Data.Any())
        {
            response.Data.AddRange(
                result.Data
                    .Select(r =>
                        r.Adapt<ProductItem>()
                    )
            );
        }

        return response;
    }
}