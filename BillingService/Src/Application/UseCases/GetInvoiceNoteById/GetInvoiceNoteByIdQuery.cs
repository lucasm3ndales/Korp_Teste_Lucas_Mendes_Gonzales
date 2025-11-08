using BillingService.Application.Common;
using BillingService.Domain.ValueObjects;
using MediatR;

namespace BillingService.Application.UseCases.GetInvoiceById;

public class GetInvoiceNoteByIdQuery(Guid id): IRequest<ApiResultDto<InvoiceNoteDto>>
{
    public Guid Id { get; } = id;
}