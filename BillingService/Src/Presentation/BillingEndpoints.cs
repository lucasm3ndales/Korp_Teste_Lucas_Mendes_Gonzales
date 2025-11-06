namespace BillingService.Presentation;

public static class BillingEndpoints
{
    public static void MapBillingRoutes(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api/billing").WithTags("Billing");

    }
}