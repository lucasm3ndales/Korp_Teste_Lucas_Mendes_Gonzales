using MediatR;
using StockService.Application.Common.Dtos;

namespace StockService.Application.UseCases.CreateProduct;

public class CreateProductCommand(string code, string description, int initialStockBalance): IRequest<ApiResultDto<ProductDto>>
{
    public string Code { get; init; } = code;
    
    public string Description { get; init; } = description;

    public int InitialStockBalance { get; init; } = initialStockBalance;
}