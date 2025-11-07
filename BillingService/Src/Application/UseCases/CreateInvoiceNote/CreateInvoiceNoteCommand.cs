using BillingService.Application.Common;
using MediatR;

namespace BillingService.Application.UseCases.CreateInvoiceNote;

public class CreateInvoiceNoteCommand: IRequest<ApiResultDto<InvoiceNoteDto>>
{
    
}