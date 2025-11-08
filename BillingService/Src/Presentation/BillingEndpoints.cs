using BillingService.Application.UseCases.CloseInvoiceNote;
using BillingService.Application.UseCases.CreateInvoiceNote;
using BillingService.Application.UseCases.GetAllInvoices;
using BillingService.Application.UseCases.GetInvoiceById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BillingService.Presentation;

public static class BillingEndpoints
{
    public static void MapV1BillingRoutes(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api/v1/billing").WithTags("Billing");
        
        api.MapPost("/invoice-notes", async (
            [FromBody] CreateInvoiceNoteCommand command,
            [FromServices] IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            
            return Results.Created(
                $"/api/invoice-notes/{result.Data.Id}",
                result
            );
        });
        
        api.MapPut("/invoice-notes/close", async (
            [FromBody] CloseInvoiceNoteCommand command,
            [FromServices] IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            
            return Results.Ok(result);
        });
        
        api.MapGet("/invoice-notes/{id:guid}", async (
            Guid id, 
            [FromServices] IMediator mediator) =>
        {
            var query = new GetInvoiceNoteByIdQuery(id);
            
            var result = await mediator.Send(query);
            
            return Results.Ok(result);
        });
        
        api.MapGet("/invoice-notes", async ([FromServices] IMediator mediator) =>
        {
            var query = new GetAllInvoiceNotesQuery();
            
            var result = await mediator.Send(query);
            
            return Results.Ok(result);
        });
    }
}