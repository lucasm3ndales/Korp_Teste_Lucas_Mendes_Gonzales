using MediatR;
using Microsoft.AspNetCore.Mvc;
using StockService.Application.UseCases.CreateProduct;
using StockService.Application.UseCases.DecreaseStockBalance;
using StockService.Application.UseCases.GetAllProducts;
using StockService.Application.UseCases.GetProductById;

namespace StockService.Presentation;

public static class ProductEndpoints
{
    public static void MapProductRoutes(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api/products").WithTags("Products");

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

        api.MapPost("/stock/decrease", async (
            [FromBody] DecreaseStockBalanceCommand command,
            [FromServices] IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Ok(result);
        });

        api.MapGet("/{id:guid}", async (
            Guid id,
            [FromServices] IMediator mediator) =>
        {
            var query = new GetProductByIdQuery(id);
            return await mediator.Send(query);
        });


        api.MapGet("/", async ([FromServices] IMediator mediator) =>
        {
            var query = new GetAllProductsQuery();
            return await mediator.Send(query);
        });
    }
}