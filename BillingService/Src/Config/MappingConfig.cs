using BillingService.Application.Common;
using BillingService.Domain.Entities;
using BillingService.Domain.ValueObjects;
using Mapster;

namespace BillingService.Config;

public class MappingConfig: IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<InvoiceNoteId, Guid>()
            .MapWith(id => id.Value);
        
        config.NewConfig<InvoiceNote, InvoiceNoteDto>()
            .Map(dest => dest.Id, src => src.Id.Value) 
            .Map(dest => dest.Status, src => src.Status.ToString());
    }
}