using BillingService.Application.Common.Exceptions;
using BillingService.Application.Common.Repositories;
using BillingService.Application.UseCases.GetInvoiceById;
using BillingService.Domain.Entities;
using BillingService.Domain.ValueObjects;
using Moq;

namespace BillingService.Application.Unit.Tests;

public class GetInvoiceNoteByIdQueryHandlerUnitTests
{
    private readonly Mock<IInvoiceNoteRepository> _invoiceNoteRepositoryMock;
    private readonly GetInvoiceNoteByIdQueryHandler _handler;
    
    public GetInvoiceNoteByIdQueryHandlerUnitTests()
    {
        _invoiceNoteRepositoryMock = new Mock<IInvoiceNoteRepository>();
        _handler = new GetInvoiceNoteByIdQueryHandler(_invoiceNoteRepositoryMock.Object);
    }
    
    [Fact(DisplayName = "Deve retornar sucesso e o DTO quando a nota fiscal é encontrada")]
    public async Task Should_ReturnSuccessAndDto_When_InvoiceNoteIsFound()
    {
        // Arrange
        var noteId = Guid.NewGuid();
        var query = new GetInvoiceNoteByIdQuery(noteId);
        var invoiceNote = new InvoiceNote();
        
        invoiceNote
            .GetType()
            .GetProperty(
                nameof(InvoiceNote.Id))?.SetValue(invoiceNote, 
                InvoiceNoteId.From(noteId));


        _invoiceNoteRepositoryMock.Setup(r => r.GetByIdWithItems(
                noteId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoiceNote);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(noteId, result.Data.Id);

        _invoiceNoteRepositoryMock.Verify(r => r.GetByIdWithItems(
            noteId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Deve lançar InvoiceNoteNotExistsException quando a nota fiscal não for encontrada")]
    public async Task Should_Throw_InvoiceNoteNotExistsException_When_InvoiceNoteIsNotFound()
    {
        // Arrange
        var noteId = Guid.NewGuid();
        var query = new GetInvoiceNoteByIdQuery(noteId);

        _invoiceNoteRepositoryMock.Setup(r => r.GetByIdWithItems(
                noteId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((InvoiceNote)null!);

        // Act & Assert
        await Assert.ThrowsAsync<InvoiceNoteNotExistsException>(() =>
            _handler.Handle(query, CancellationToken.None));
        
        _invoiceNoteRepositoryMock.Verify(r => r.GetByIdWithItems(
            noteId, It.IsAny<CancellationToken>()), Times.Once);
    }
}