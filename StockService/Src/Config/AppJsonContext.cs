using System.Text.Json.Serialization;
using StockService.Application.Common.Dtos;
using StockService.Application.UseCases.CreateProduct;
using StockService.Application.UseCases.DecreaseStockBalance;
using StockService.Application.UseCases.GetAllProducts;
using StockService.Application.UseCases.GetProductById;

namespace StockService.Config;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(CreateProductCommand))]
[JsonSerializable(typeof(DecreaseStockProductsInBatchCommand))]
[JsonSerializable(typeof(GetAllProductsQuery))]
[JsonSerializable(typeof(GetProductByIdQuery))]
[JsonSerializable(typeof(ApiResultDto<ProductDto>))] 
[JsonSerializable(typeof(ApiResultDto<IEnumerable<ProductDto>>))]
public partial class AppJsonContext: JsonSerializerContext
{
}