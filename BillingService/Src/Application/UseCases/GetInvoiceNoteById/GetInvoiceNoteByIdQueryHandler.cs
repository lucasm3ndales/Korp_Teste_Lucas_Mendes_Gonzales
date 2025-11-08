using BillingService.Application.Common;
using BillingService.Application.Common.Exceptions;
using BillingService.Application.Common.Repositories;
using Mapster;
using MediatR;

namespace BillingService.Application.UseCases.GetInvoiceById;

public class GetInvoiceNoteByIdQueryHandler(
    IInvoiceNoteRepository invoiceNoteRepository
) : IRequestHandler<GetInvoiceNoteByIdQuery, ApiResultDto<InvoiceNoteDto>>
{
    public async Task<ApiResultDto<InvoiceNoteDto>> Handle(GetInvoiceNoteByIdQuery request,
        CancellationToken cancellationToken)
    {
        var invoiceNote = await invoiceNoteRepository.GetByIdWithItems(request.Id, cancellationToken);

        if (invoiceNote == null)
            throw new InvoiceNoteNotExistsException();

        return ApiResultDto<InvoiceNoteDto>.Success(invoiceNote.Adapt<InvoiceNoteDto>());
    }
}