using BillingService.Application.Common;
using MediatR;

namespace BillingService.Application.UseCases.GetAllInvoices;

public class GetAllInvoiceNotesQuery: IRequest<ApiResultDto<IEnumerable<InvoiceNoteDto>>>
{
    
}