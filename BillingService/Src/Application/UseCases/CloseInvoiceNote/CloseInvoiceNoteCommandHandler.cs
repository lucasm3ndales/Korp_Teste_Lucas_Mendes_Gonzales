using BillingService.Application.Common;
using BillingService.Application.Common.Repositories;
using MediatR;

namespace BillingService.Application.UseCases.CloseInvoiceNote;

public class CloseInvoiceNoteCommandHandler(
    IInvoiceNoteRepository invoiceNoteRepository
    ): IRequestHandler<CloseInvoiceNoteCommand, ApiResultDto<bool>>
{
    public Task<ApiResultDto<bool>> Handle(CloseInvoiceNoteCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}