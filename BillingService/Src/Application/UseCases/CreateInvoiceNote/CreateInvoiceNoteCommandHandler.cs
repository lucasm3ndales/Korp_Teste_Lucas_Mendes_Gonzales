using BillingService.Application.Common;
using BillingService.Application.Common.Exceptions;
using BillingService.Application.Common.Repositories;
using BillingService.Domain.Entities;
using Mapster;
using MediatR;

namespace BillingService.Application.UseCases.CreateInvoiceNote;

public class CreateInvoiceNoteCommandHandler(
    IInvoiceNoteRepository invoiceNoteRepository
    ): IRequestHandler<CreateInvoiceNoteCommand, ApiResultDto<InvoiceNoteDto>>
{
    public async Task<ApiResultDto<InvoiceNoteDto>> Handle(
        CreateInvoiceNoteCommand request, 
        CancellationToken cancellationToken)
    {
        var lastNumberNote = await invoiceNoteRepository.GetLastSequentialNumber(cancellationToken);

        var invoiceNote = new InvoiceNote(lastNumberNote + 1);
        
        foreach (var i in request.Items)
        {
            invoiceNote.AddItem(
                i.ProductId,
                i.ProductCode,
                i.ProductDescription,
                i.Quantity
            );
        }

        await invoiceNoteRepository.Add(invoiceNote, cancellationToken);
        await invoiceNoteRepository.SaveChanges(cancellationToken);

        var invoiceNoteDto = invoiceNote.Adapt<InvoiceNoteDto>();

        return ApiResultDto<InvoiceNoteDto>.Success("Nota fiscal criada com sucesso!", invoiceNoteDto);
    }
}