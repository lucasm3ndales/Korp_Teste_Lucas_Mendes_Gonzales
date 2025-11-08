using BillingService.Application.Common;
using BillingService.Application.Common.Exceptions;
using BillingService.Application.Common.Repositories;
using BillingService.Domain.Entities;
using BillingService.Domain.Enums;
using BillingService.Domain.Exceptions;
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

        if (invoiceNote.Status == InvoiceNoteStatus.PROCESSING)
            throw new InvoiceNoteConcurrencyException();

        if (invoiceNote.Status != InvoiceNoteStatus.OPEN)
            throw new InvalidInvoiceNoteStatusException(invoiceNote.Status);

        await ChangeInvoiceNoteStatusToProcessing(invoiceNote, cancellationToken);

        var items = invoiceNote.Items.Select(i => new StockItem
        {
            ProductId = i.ProductId.ToString(),
            Quantity = i.Quantity
        });

        var grpcRequest = new DecreaseStockProductsInBatchRequest();
        grpcRequest.Items.AddRange(items);

        var grpcResponse = await SendDecreaseStockProductsInBatchRequest(
            invoiceNote,
            grpcRequest,
            cancellationToken
        );

        if (!grpcResponse.IsSuccess)
        {
            await RevertInvoiceStatusToOpen(invoiceNote, cancellationToken);
            throw new DecreaseStockProductsInBatchException(grpcResponse.Messages.ToList());
        }

        invoiceNote.Close();
        await invoiceNoteRepository.SaveChanges(cancellationToken);

        return ApiResultDto<bool>.Success("Nota fiscal fechada com sucesso!", true);
    }

    private async Task<DecreaseStockProductsInBatchResponse> SendDecreaseStockProductsInBatchRequest(
        InvoiceNote invoiceNote,
        DecreaseStockProductsInBatchRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            return await grpcClient.DecreaseStockProductsInBatchAsync(request);
        }
        catch (Exception ex)
        {
            await RevertInvoiceStatusToOpen(invoiceNote, cancellationToken);
            throw new ApplicationException($"Falha ao comunicar com o serviço de estoque: {ex.Message}", ex);
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
}