using BillingService.Application.Common;
using MediatR;

namespace BillingService.Application.UseCases.GetInvoiceById;

public class GetInvoiceNoteByIdQuery: IRequest<ApiResultDto<InvoiceNoteDto>>
{
    
}