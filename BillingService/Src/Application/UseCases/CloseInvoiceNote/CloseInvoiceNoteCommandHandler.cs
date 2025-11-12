using BillingService.Application.Common;
using BillingService.Application.Common.Exceptions;
using BillingService.Application.Common.Repositories;
using BillingService.Domain.Entities;
using BillingService.Domain.Enums;
using BillingService.Domain.Exceptions;
using Grpc.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StockManager.Grpc;

namespace BillingService.Application.UseCases.CloseInvoiceNote;

public class CloseInvoiceNoteCommandHandler(
    IInvoiceNoteRepository invoiceNoteRepository,
    StockManager.Grpc.StockManager.StockManagerClient grpcClient
) : IRequestHandler<CloseInvoiceNoteCommand, ApiResultDto<bool>>
{
    public async Task<ApiResultDto<bool>> Handle(CloseInvoiceNoteCommand request, CancellationToken cancellationToken)
    {
        var invoiceNote = await invoiceNoteRepository.GetByIdWithItems(request.Id, cancellationToken);

        if (invoiceNote == null)
            throw new InvoiceNoteNotExistsException();

        if (invoiceNote.Status == InvoiceNoteStatus.CLOSED)
            return ApiResultDto<bool>.Success("A nota fiscal já esta fechada.", true);
        
        if (invoiceNote.Status != InvoiceNoteStatus.OPEN && !request.IsSyncProcess)
            throw new InvalidInvoiceNoteStatusException(invoiceNote.Status);
        
        if (invoiceNote.Xmin != request.Xmin)
            throw new InvoiceNoteConcurrencyException();

        var productIds = invoiceNote
            .Items
            .Select(i => i.ProductId.ToString())
            .Distinct()
            .ToList();
        
        var products = await GetProductsByIds(productIds);
        
        var productXminDic = products.ToDictionary(p => p.Id, p => p.Xmin);
        
        await ChangeInvoiceNoteStatusToProcessing(invoiceNote, cancellationToken);

        var items = invoiceNote.Items.Select(i => new StockItem
        {
            ProductId = i.ProductId.ToString(),
            Quantity = i.Quantity,
            Xmin = productXminDic.GetValueOrDefault(i.ProductId.ToString())
        });

        var grpcRequest = new DecreaseStockProductsInBatchRequest();
        
        grpcRequest.InvoiceNoteId = invoiceNote.Id.ToString();
        grpcRequest.Items.AddRange(items);

        await SendDecreaseStockProductsInBatchRequest(
            invoiceNote,
            grpcRequest,
            cancellationToken
        );

        invoiceNote.Close();
        invoiceNote.SetModified();
        await invoiceNoteRepository.SaveChanges(cancellationToken);

        return ApiResultDto<bool>.Success("Nota fiscal fechada com sucesso!", true);
    }

    private async Task SendDecreaseStockProductsInBatchRequest(
        InvoiceNote invoiceNote,
        DecreaseStockProductsInBatchRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await grpcClient.DecreaseStockProductsInBatchAsync(request);
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
        {
            throw new ServiceUnavailableException("Estoque", ex);
        }
        catch (RpcException ex)
        {
            await RevertInvoiceStatusToOpen(invoiceNote, cancellationToken);
            throw new DecreaseStockProductsInBatchException(ex.Status.Detail);
        }
    }

    private async Task ChangeInvoiceNoteStatusToProcessing(
        InvoiceNote invoiceNote,
        CancellationToken cancellationToken
    )
    {
        try
        {
            invoiceNote.Process();
            await invoiceNoteRepository.SaveChanges(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvoiceNoteConcurrencyException();
        }
    }

    private async Task RevertInvoiceStatusToOpen(
        InvoiceNote invoiceNote,
        CancellationToken cancellationToken
    )
    {
        invoiceNote.RevertToOpen();
        await invoiceNoteRepository.SaveChanges(cancellationToken);
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