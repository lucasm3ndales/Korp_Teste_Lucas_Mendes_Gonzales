using MediatR;
using Microsoft.AspNetCore.Mvc;
using StockService.Application.UseCases.CreateProduct;
using StockService.Application.UseCases.DecreaseStockBalance;
using StockService.Application.UseCases.GetAllProducts;
using StockService.Application.UseCases.GetProductByProductId;

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
                $"/api/products/{result.Data.ProductId}",
                result
            );
        });

        api.MapPost("/{id:guid}/stock/decrease", async (
            Guid id,
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
            var query = new GetProductByProductIdQuery(id);
            return await mediator.Send(query);
        });


        api.MapGet("/", async ([FromServices] IMediator mediator) =>
        {
            var query = new GetAllProductsQuery();
            return await mediator.Send(query);
        });
    }
}