using BillingService.Application.Common.Exceptions;
using BillingService.Application.Common.Repositories;
using BillingService.Application.UseCases.CreateInvoiceNote;
using BillingService.Domain.Entities;
using Grpc.Core;
using Moq;
using StockManager.Grpc;

namespace BillingService.Application.Unit.Tests;

public class CreateInvoiceNoteCommandHandlerUnitTests
{
    private readonly Mock<IInvoiceNoteRepository> _invoiceNoteRepositoryMock;
    private readonly Mock<StockManager.Grpc.StockManager.StockManagerClient> _grpcClientMock;
    private readonly CreateInvoiceNoteCommandHandler _handler;

    public CreateInvoiceNoteCommandHandlerUnitTests()
    {
        _invoiceNoteRepositoryMock = new Mock<IInvoiceNoteRepository>();
        _grpcClientMock = new Mock<StockManager.Grpc.StockManager.StockManagerClient>();

        _handler = new CreateInvoiceNoteCommandHandler(
            _invoiceNoteRepositoryMock.Object,
            _grpcClientMock.Object
        );
    }

    [Fact(DisplayName = "Deve retornar sucesso e criar a nota fiscal quando produtos forem encontrados")]
    public async Task Should_ReturnSuccessAndCreateInvoiceNote_When_ProductsAreFound()
    {
        // Arrange
        var productId1 = Guid.NewGuid();
        var productId2 = Guid.NewGuid();

        var command = new CreateInvoiceNoteCommand
        {
            Items =
            [
                new CreateInvoiceNoteItemCommand { ProductId = productId1, Quantity = 5 },
                new CreateInvoiceNoteItemCommand { ProductId = productId2, Quantity = 10 }
            ]
        };

        var grpcResponse = new GetProductsByIdsResponse
        {
            IsSuccess = true,
            Data =
            {
                new ProductItem { Id = productId1.ToString(), Code = "A1", Description = "Prod A" },
                new ProductItem { Id = productId2.ToString(), Code = "B2", Description = "Prod B" }
            }
        };

        _grpcClientMock
            .Setup(c => c.GetProductsByIdsAsync(It.IsAny<GetProductsByIdsRequest>(), It.IsAny<CallOptions>()))
            .Returns(CreateAsyncUnaryCall(grpcResponse));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Items.Count());

        _invoiceNoteRepositoryMock.Verify(r => r.Add(
            It.Is<InvoiceNote>(n => n.Items.Count == 2),
            It.IsAny<CancellationToken>()), Times.Once);

        _invoiceNoteRepositoryMock.Verify(r => r.SaveChanges(
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Deve lançar InvoiceNoteItemsEmptyException quando a lista de itens for vazia")]
    public async Task Should_Throw_InvoiceNoteItemsEmptyException_When_ItemsListIsEmpty()
    {
        var command = new CreateInvoiceNoteCommand { Items = [] };

        await Assert.ThrowsAsync<InvoiceNoteItemsEmptyException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _grpcClientMock.Verify(c => c.GetProductsByIdsAsync(
            It.IsAny<GetProductsByIdsRequest>(),
            It.IsAny<CallOptions>()), Times.Never);
    }

    [Fact(DisplayName =
        "Deve lançar InvoiceNoteProductsNotFoundException quando o gRPC não encontrar um ou mais produtos")]
    public async Task Should_Throw_InvoiceNoteProductsNotFoundException_When_GrpcDoesNotReturnAllProducts()
    {
        var productId1 = Guid.NewGuid();
        var productId2 = Guid.NewGuid();

        var command = new CreateInvoiceNoteCommand
        {
            Items =
            [
                new CreateInvoiceNoteItemCommand { ProductId = productId1, Quantity = 5 },
                new CreateInvoiceNoteItemCommand { ProductId = productId2, Quantity = 10 }
            ]
        };

        var grpcResponse = new GetProductsByIdsResponse
        {
            IsSuccess = true,
            Data = { new ProductItem { Id = productId1.ToString(), Code = "A1", Description = "Prod A" } }
        };

        _grpcClientMock
            .Setup(c => c.GetProductsByIdsAsync(It.IsAny<GetProductsByIdsRequest>(), It.IsAny<CallOptions>()))
            .Returns(CreateAsyncUnaryCall(grpcResponse));

        await Assert.ThrowsAsync<InvoiceNoteProductsNotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact(DisplayName = "Deve lançar ServiceUnavailableException quando o serviço gRPC estiver indisponível")]
    public async Task Should_Throw_ServiceUnavailableException_When_GrpcServiceIsUnavailable()
    {
        var command = new CreateInvoiceNoteCommand
        {
            Items = [new CreateInvoiceNoteItemCommand { ProductId = Guid.NewGuid(), Quantity = 5 }]
        };

        _grpcClientMock
            .Setup(c => c.GetProductsByIdsAsync(It.IsAny<GetProductsByIdsRequest>(), It.IsAny<CallOptions>()))
            .Throws(new RpcException(new Status(StatusCode.Unavailable, "Service is down")));

        await Assert.ThrowsAsync<ServiceUnavailableException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact(DisplayName = "Deve lançar InvoiceNoteProductsNotFoundException quando a resposta gRPC for IsSuccess=false")]
    public async Task Should_Throw_InvoiceNoteProductsNotFoundException_When_GrpcResponseIsNotSuccess()
    {
        var command = new CreateInvoiceNoteCommand
        {
            Items = [new CreateInvoiceNoteItemCommand { ProductId = Guid.NewGuid(), Quantity = 5 }]
        };

        var grpcResponse = new GetProductsByIdsResponse
        {
            IsSuccess = false,
            Messages = { "Erro de validação do estoque" }
        };

        _grpcClientMock
            .Setup(c => c.GetProductsByIdsAsync(It.IsAny<GetProductsByIdsRequest>(), It.IsAny<CallOptions>()))
            .Returns(CreateAsyncUnaryCall(grpcResponse));

        await Assert.ThrowsAsync<InvoiceNoteProductsNotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }
    
    private static AsyncUnaryCall<T> CreateAsyncUnaryCall<T>(T response) where T : class
    {
        var responseTask = Task.FromResult(response);

        // Nenhum desses pode ser null
        return new AsyncUnaryCall<T>(
            responseTask,
            Task.FromResult(new Metadata()),
            () => Status.DefaultSuccess,
            () => new Metadata(),
            () => { }
        );
    }
}