using MediatR;
using StockService.Application.Common.Dtos;

namespace StockService.Application.UseCases.CreateProduct;

public class CreateProductCommand: IRequest<ApiResultDto<ProductDto>>
{
    public string Code { get; init; }
    
    public string Description { get; init; }

    public int InitialStockBalance { get; init; }
}