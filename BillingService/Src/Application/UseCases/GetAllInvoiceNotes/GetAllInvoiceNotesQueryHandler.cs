using BillingService.Application.Common;
using BillingService.Application.Common.Repositories;
using Mapster;
using MediatR;

namespace BillingService.Application.UseCases.GetAllInvoices;

public class GetAllInvoiceNotesQueryHandler(
    IInvoiceNoteRepository invoiceNoteRepository
) : IRequestHandler<GetAllInvoiceNotesQuery, ApiResultDto<IEnumerable<InvoiceNoteDto>>>
{
    public async Task<ApiResultDto<IEnumerable<InvoiceNoteDto>>> Handle(GetAllInvoiceNotesQuery request,
        CancellationToken cancellationToken)
    {
        var invoiceNotes = await invoiceNoteRepository.ListAllNoTracked(cancellationToken);

        if (!invoiceNotes.Any())
            return ApiResultDto<IEnumerable<InvoiceNoteDto>>.Success([]);

        return ApiResultDto<IEnumerable<InvoiceNoteDto>>.Success(invoiceNotes
            .Select(i => i.Adapt<InvoiceNoteDto>())
        );
    }
}