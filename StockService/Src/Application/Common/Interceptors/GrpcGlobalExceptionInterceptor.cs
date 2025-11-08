using Grpc.Core;
using Grpc.Core.Interceptors;

using StockService.Application.Common.Exceptions;
using StockService.Domain.Exceptions;

namespace StockService.Application.Common.Interceptors;

public class GrpcGlobalExceptionInterceptor(
    ILogger<GrpcGlobalExceptionInterceptor> logger
) : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro capturado pelo gRPC Interceptor: {Message}", ex.Message);
            throw MapToRpcException(ex);
        }
    }

    private RpcException MapToRpcException(Exception exception)
    {
        switch (exception)
        {
            case StockConcurrencyException:
                return new RpcException(new Status(StatusCode.Aborted, exception.Message));

            case ProductCodeAlreadyExistsException:
                return new RpcException(new Status(StatusCode.AlreadyExists, exception.Message));

            case ProductNotExistsException:
                return new RpcException(new Status(StatusCode.NotFound, exception.Message));
                
            case InsufficientStockBalanceException:
                return new RpcException(new Status(StatusCode.FailedPrecondition, exception.Message));

            case InvalidProductIdException:
            case InvalidProductCodeException:
            case InvalidProductDescriptionException:
            case InvalidProductStockBalanceException:
            case InvalidStockBalanceQuantityException:
                return new RpcException(new Status(StatusCode.InvalidArgument, exception.Message));

            case StockDomainException:
            case StockApplicationException:
                return new RpcException(new Status(StatusCode.FailedPrecondition, exception.Message));

            default:
                return new RpcException(new Status(StatusCode.Internal, $"Erro interno inesperado no serviço de estoque: {exception.Message}"));
        }
    }
}