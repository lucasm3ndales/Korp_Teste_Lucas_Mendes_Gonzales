using System.Net;
using System.Text.Json;
using StockService.Application.Common.Dtos;
using StockService.Application.Common.Exceptions;
using StockService.Domain.Exceptions;

namespace StockService.Application.Common.Middlewares;

public class HttpGlobalExceptionMiddleware(
    RequestDelegate next,
    ILogger<HttpGlobalExceptionMiddleware> logger
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
            case StockConcurrencyException:
            case ProductCodeAlreadyExistsException:
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                apiResult = ApiResultDto<object>.Failure(exception.Message);
                break;

            case ProductNotExistsException:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                apiResult = ApiResultDto<object>.Failure(exception.Message);
                break;

            case InvalidProductIdException:
            case InsufficientStockBalanceException:
            case InvalidProductCodeException:
            case InvalidProductDescriptionException:
            case InvalidProductStockBalanceException:
            case InvalidStockBalanceQuantityException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                apiResult = ApiResultDto<object>.Failure(exception.Message);
                break;

            case StockDomainException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                apiResult = ApiResultDto<object>.Failure(exception.Message);
                break;

            case StockApplicationException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                apiResult = ApiResultDto<object>.Failure(exception.Message);
                break;

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