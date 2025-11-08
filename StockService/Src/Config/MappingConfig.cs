using Mapster;
using StockService.Application.Common.Dtos;
using StockService.Domain.Entities;
using StockService.Domain.ValueObjects;

namespace StockService.Config;

public class MappingConfig: IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ProductId, Guid>()
            .MapWith(id => id.Value);
              
        config.NewConfig<Product, ProductDto>()
            .Map(dest => dest.Id, src => src.Id.Value);
    }
}