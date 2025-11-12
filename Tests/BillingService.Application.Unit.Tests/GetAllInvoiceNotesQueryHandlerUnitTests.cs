using BillingService.Application.Common.Repositories;
using BillingService.Application.UseCases.GetAllInvoices;
using BillingService.Domain.Entities;
using BillingService.Domain.ValueObjects;
using Moq;

namespace BillingService.Application.Unit.Tests;

public class GetAllInvoiceNotesQueryHandlerUnitTests
{
    private readonly Mock<IInvoiceNoteRepository> _invoiceNoteRepositoryMock;
    private readonly GetAllInvoiceNotesQueryHandler _handler;

    public GetAllInvoiceNotesQueryHandlerUnitTests()
    {
        _invoiceNoteRepositoryMock = new Mock<IInvoiceNoteRepository>();
        _handler = new GetAllInvoiceNotesQueryHandler(_invoiceNoteRepositoryMock.Object);
    }

    [Fact(DisplayName = "Deve retornar sucesso com lista de notas fiscais quando existirem notas")]
    public async Task Should_ReturnSuccessWithInvoiceNoteList_When_InvoiceNotesExist()
    {
        // Arrange
        var query = new GetAllInvoiceNotesQuery();

        var note1 = new InvoiceNote();
        note1
            .GetType()
            .GetProperty(
                nameof(InvoiceNote.Id))?.SetValue(note1,
                InvoiceNoteId.NewId()
            );

        var note2 = new InvoiceNote();
        note2
            .GetType()
            .GetProperty(
                nameof(InvoiceNote.Id))?.SetValue(
                note2,
                InvoiceNoteId.NewId()
            );

        var invoiceNoteList = new List<InvoiceNote> { note1, note2 };

        _invoiceNoteRepositoryMock.Setup(r => r.ListAllNoTracked(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoiceNoteList);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count());
    }

    [Fact(DisplayName = "Deve retornar sucesso com lista vazia quando não existirem notas fiscais")]
    public async Task Should_ReturnSuccessWithEmptyList_When_NoInvoiceNotesExist()
    {
        // Arrange
        var query = new GetAllInvoiceNotesQuery();
        var emptyList = new List<InvoiceNote>();

        _invoiceNoteRepositoryMock.Setup(r => r.ListAllNoTracked(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyList);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data);
    }
}