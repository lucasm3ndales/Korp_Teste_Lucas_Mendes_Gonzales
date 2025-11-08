using System.Net;
using System.Text.Json;
using BillingService.Application.Common.Exceptions;
using BillingService.Domain.Exceptions;

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
            case InvoiceNoteNotExistsException:
                context. Response.StatusCode = (int)HttpStatusCode.NotFound;
                apiResult = ApiResultDto<object>.Failure(exception.Message);
                break;
            
            case InvoiceNoteProductsNotFoundException ex:
                context. Response.StatusCode = (int)HttpStatusCode.NotFound;
                apiResult = ApiResultDto<object>.Failure(ex.ErrorMessages);
                break;

            case InvalidInvoiceNoteStatusException:
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                apiResult = ApiResultDto<object>.Failure(exception.Message);
                break;

            case DecreaseStockProductsInBatchException ex:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                apiResult = ApiResultDto<object>.Failure(ex.ErrorMessages);
                break;

            case InvoiceNoteEmptyException:
            case InvoiceNoteItemsEmptyException:
            case InvalidInvoiceNoteIdException:
            case InvalidInvoiceNoteItemProductException:
            case InvalidInvoiceNoteItemQuantityException:
            case BillingDomainException:
            case BillingApplicationException:
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