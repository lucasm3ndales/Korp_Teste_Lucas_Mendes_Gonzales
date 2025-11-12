using MediatR;
using Microsoft.AspNetCore.Mvc;
using StockService.Application.UseCases.CreateProduct;
using StockService.Application.UseCases.GetAllProducts;

namespace StockService.Presentation.Http;

public static class StockEndpoints
{
    public static void MapV1ProductRoutes(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api/stock/v1/products").WithTags("Products");

        api.MapPost("/", async (
            [FromBody] CreateProductCommand command,
            [FromServices] IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Created(
                $"/api/products/{result.Data.Id}",
                result
            );
        });
        
        api.MapGet("/", async ([FromServices] IMediator mediator) =>
        {
            var query = new GetAllProductsQuery();
            return await mediator.Send(query);
        });
    }
}