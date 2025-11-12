using BillingService.Application.Common;
using BillingService.Application.Common.Exceptions;
using BillingService.Application.Common.Repositories;
using BillingService.Domain.Entities;
using Grpc.Core;
using Mapster;
using MediatR;
using StockManager.Grpc;

namespace BillingService.Application.UseCases.CreateInvoiceNote;

public class CreateInvoiceNoteCommandHandler(
    IInvoiceNoteRepository invoiceNoteRepository,
    StockManager.Grpc.StockManager.StockManagerClient grpcClient
) : IRequestHandler<CreateInvoiceNoteCommand, ApiResultDto<InvoiceNoteDto>>
{
    public async Task<ApiResultDto<InvoiceNoteDto>> Handle(
        CreateInvoiceNoteCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Items == null || !request.Items.Any())
            throw new InvoiceNoteItemsEmptyException();

        var productIds = request.Items
            .Select(i => i.ProductId.ToString())
            .ToList();

        var products = await GetProductsByIds(productIds);

        var productMap = products.ToDictionary(p => Guid.Parse(p.Id));
        
        var invoiceNote = new InvoiceNote();

        foreach (var item in request.Items)
        {
            if (!productMap.TryGetValue(item.ProductId, out var productDetails))
            {
                throw new InvoiceNoteProductsNotFoundException();
            }
            
            invoiceNote.AddItem(
                item.ProductId,
                productDetails.Code,
                productDetails.Description,
                item.Quantity
            );
        }

        await invoiceNoteRepository.Add(invoiceNote, cancellationToken);
        
        await invoiceNoteRepository.SaveChanges(cancellationToken);

        return ApiResultDto<InvoiceNoteDto>.Success(
            "Nota fiscal criada com sucesso!",
            invoiceNote.Adapt<InvoiceNoteDto>());
    }

    private async Task<IEnumerable<ProductItem>> GetProductsByIds(List<string> productIds)
    {
        try
        {
            var grpcRequest = new GetProductsByIdsRequest();
            grpcRequest.ProductIds.AddRange(productIds);

            var products = await grpcClient.GetProductsByIdsAsync(grpcRequest);

            if (!products.IsSuccess
                || products.Data == null
                || !products.Data.Any())
                throw new InvoiceNoteProductsNotFoundException(products.Messages.ToList());

            return products.Data;
            
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
        {
            throw new ServiceUnavailableException("Estoque", ex);
        }
    }
}