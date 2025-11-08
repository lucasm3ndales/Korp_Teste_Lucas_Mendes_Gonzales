using BillingService.Application.Common;
using BillingService.Application.Common.Repositories;
using MediatR;

namespace BillingService.Application.UseCases.GetInvoiceById;

public class GetInvoiceNoteByIdQueryHandler(
    IInvoiceNoteRepository invoiceNoteRepository
    ): IRequestHandler<GetInvoiceNoteByIdQuery, ApiResultDto<InvoiceNoteDto>>
{
    public Task<ApiResultDto<InvoiceNoteDto>> Handle(GetInvoiceNoteByIdQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}