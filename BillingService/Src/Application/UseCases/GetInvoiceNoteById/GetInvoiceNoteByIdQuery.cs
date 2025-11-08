using BillingService.Application.Common;
using BillingService.Domain.ValueObjects;
using MediatR;

namespace BillingService.Application.UseCases.GetInvoiceById;

public class GetInvoiceNoteByIdQuery(string id): IRequest<ApiResultDto<InvoiceNoteDto>>
{
    public InvoiceNoteId Id { get; }
}