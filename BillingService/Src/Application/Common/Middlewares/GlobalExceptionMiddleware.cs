using System.Net;
using System.Text.Json;

namespace BillingService.Application.Common.Middlewares;

public class GlobalExceptionMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionMiddleware> logger
)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        ApiResultDto<object> apiResult;

        switch (exception)
        {
            default:
                logger.LogError(exception, "Erro não tratado: {Message}", exception.Message);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                apiResult = ApiResultDto<object>.Failure("Ocorreu um erro inesperado no servidor.");
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(apiResult);
        await context.Response.WriteAsync(jsonResponse);
    }
}