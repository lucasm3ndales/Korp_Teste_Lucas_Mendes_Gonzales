using BillingService.Application.Common;
using BillingService.Application.Common.Repositories;
using MediatR;

namespace BillingService.Application.UseCases.GetAllInvoices;

public class GetAllInvoiceNotesQueryHandler(
    IInvoiceNoteRepository invoiceNoteRepository
    ): IRequestHandler<GetAllInvoiceNotesQuery, ApiResultDto<IEnumerable<InvoiceNoteDto>>>
{
    public Task<ApiResultDto<IEnumerable<InvoiceNoteDto>>> Handle(GetAllInvoiceNotesQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}