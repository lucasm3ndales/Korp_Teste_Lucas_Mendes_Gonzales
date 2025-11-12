using BillingService.Application.Common;
using MediatR;  

namespace BillingService.Application.UseCases.CloseInvoiceNote;

public class CloseInvoiceNoteCommand: IRequest<ApiResultDto<bool>>
{
    public Guid Id { get; init; }
}