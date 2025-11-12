using BillingService.Domain.Entities;
using BillingService.Domain.Enums;
using BillingService.Domain.Exceptions;

namespace BillingService.Domain.Unit.Tests;

public class InvoiceNoteUnitTests
{
    [Fact(DisplayName = "Deve criar uma InvoiceNote com status OPEN e campos inicializados")]
    public void Should_CreateInvoiceNote_WithOpenStatusAndInitializedFields()
    {
        // Arrange & Act
        var invoiceNote = new InvoiceNote();

        // Assert
        Assert.NotNull(invoiceNote.Id);
        Assert.Equal(InvoiceNoteStatus.OPEN, invoiceNote.Status);
        Assert.NotNull(invoiceNote.CreatedAt);
        Assert.NotNull(invoiceNote.UpdatedAt);
        Assert.Empty(invoiceNote.Items);
    }

    [Fact(DisplayName = "Deve adicionar um item à lista quando o status for OPEN")]
    public void Should_AddItem_When_StatusIsOpen()
    {
        // Arrange
        var invoiceNote = new InvoiceNote();
        var productId = Guid.NewGuid();
        var code = "PRD-001";
        var description = "Produto A";
        var quantity = 5;

        // Act
        invoiceNote.AddItem(productId, code, description, quantity);

        // Assert
        Assert.Single(invoiceNote.Items);
        Assert.Equal(code, invoiceNote.Items.First().ProductCode);
        Assert.Equal(quantity, invoiceNote.Items.First().Quantity);
    }

    [Theory(DisplayName = "Deve lançar InvalidInvoiceNoteStatusException ao adicionar item se o status não for OPEN")]
    [InlineData(InvoiceNoteStatus.PROCESSING)]
    [InlineData(InvoiceNoteStatus.CLOSED)]
    public void Should_Throw_InvalidInvoiceNoteStatusException_When_AddingItemAndStatusIsNotOpen(
        InvoiceNoteStatus status)
    {
        // Arrange
        var invoiceNote = new InvoiceNote();

        if (status == InvoiceNoteStatus.PROCESSING)
        {
            invoiceNote.Process();
        }
        else if (status == InvoiceNoteStatus.CLOSED)
        {
            invoiceNote.AddItem(Guid.NewGuid(), "A", "B", 1);
            invoiceNote.Process();
            invoiceNote.Close();
        }

        // Act & Assert
        Assert.Throws<InvalidInvoiceNoteStatusException>(() =>
            invoiceNote.AddItem(Guid.NewGuid(), "PRD-002", "Produto B", 1));
    }

    [Fact(DisplayName = "Deve mudar o status de OPEN para PROCESSING ao chamar Process")]
    public void Should_ChangeStatusFromOpenToProcessing_When_CallingProcess()
    {
        // Arrange
        var invoiceNote = new InvoiceNote();

        // Act
        invoiceNote.Process();

        // Assert
        Assert.Equal(InvoiceNoteStatus.PROCESSING, invoiceNote.Status);
    }

    [Theory(DisplayName = "Deve lançar InvalidInvoiceNoteStatusException ao chamar Process se o status não for OPEN")]
    [InlineData(InvoiceNoteStatus.PROCESSING)]
    [InlineData(InvoiceNoteStatus.CLOSED)]
    public void Should_Throw_InvalidInvoiceNoteStatusException_When_CallingProcessAndStatusIsNotOpen(
        InvoiceNoteStatus status)
    {
        // Arrange
        var invoiceNote = new InvoiceNote();

        if (status == InvoiceNoteStatus.PROCESSING)
        {
            invoiceNote.Process();
        }
        else if (status == InvoiceNoteStatus.CLOSED)
        {
            invoiceNote.AddItem(Guid.NewGuid(), "A", "B", 1);
            invoiceNote.Process();
            invoiceNote.Close();
        }

        // Act & Assert
        Assert.Throws<InvalidInvoiceNoteStatusException>(() => invoiceNote.Process());
    }

    [Fact(DisplayName = "Deve mudar o status de PROCESSING para CLOSED ao chamar Close")]
    public void Should_ChangeStatusFromProcessingToClosed_When_CallingClose()
    {
        // Arrange
        var invoiceNote = new InvoiceNote();
        invoiceNote.AddItem(Guid.NewGuid(), "A", "B", 1);
        invoiceNote.Process();

        // Act
        invoiceNote.Close();

        // Assert
        Assert.Equal(InvoiceNoteStatus.CLOSED, invoiceNote.Status);
    }

    [Theory(DisplayName =
        "Deve lançar InvalidInvoiceNoteStatusException ao chamar Close se o status não for PROCESSING")]
    [InlineData(InvoiceNoteStatus.OPEN)]
    [InlineData(InvoiceNoteStatus.CLOSED)]
    public void Should_Throw_InvalidInvoiceNoteStatusException_When_CallingCloseAndStatusIsNotProcessing(
        InvoiceNoteStatus status)
    {
        // Arrange
        var invoiceNote = new InvoiceNote();
        invoiceNote.AddItem(Guid.NewGuid(), "A", "B", 1);

        if (status == InvoiceNoteStatus.CLOSED)
        {
            invoiceNote.Process();
            invoiceNote.Close();
        }

        // Act & Assert
        Assert.Throws<InvalidInvoiceNoteStatusException>(() => invoiceNote.Close());
    }

    [Fact(DisplayName = "Deve lançar InvoiceNoteEmptyException ao chamar Close se não houver itens")]
    public void Should_Throw_InvoiceNoteEmptyException_When_CallingCloseAndItemsAreEmpty()
    {
        // Arrange
        var invoiceNote = new InvoiceNote();
        invoiceNote.Process();

        // Act & Assert
        Assert.Throws<InvoiceNoteEmptyException>(() => invoiceNote.Close());
    }

    [Fact(DisplayName = "Deve mudar o status de PROCESSING para OPEN ao chamar RevertToOpen")]
    public void Should_ChangeStatusFromProcessingToOpen_When_CallingRevertToOpen()
    {
        // Arrange
        var invoiceNote = new InvoiceNote();
        invoiceNote.Process();

        // Act
        invoiceNote.RevertToOpen();

        // Assert
        Assert.Equal(InvoiceNoteStatus.OPEN, invoiceNote.Status);
    }

    [Theory(DisplayName =
        "Deve lançar InvalidInvoiceNoteStatusException ao chamar RevertToOpen se o status não for PROCESSING")]
    [InlineData(InvoiceNoteStatus.OPEN)]
    [InlineData(InvoiceNoteStatus.CLOSED)]
    public void Should_Throw_InvalidInvoiceNoteStatusException_When_CallingRevertToOpenAndStatusIsNotProcessing(
        InvoiceNoteStatus status)
    {
        // Arrange
        var invoiceNote = new InvoiceNote();
        invoiceNote.AddItem(Guid.NewGuid(), "A", "B", 1);

        if (status == InvoiceNoteStatus.CLOSED)
        {
            invoiceNote.Process();
            invoiceNote.Close();
        }

        // Act & Assert
        Assert.Throws<InvalidInvoiceNoteStatusException>(() => invoiceNote.RevertToOpen());
    }

    [Fact(DisplayName = "Deve atualizar a propriedade UpdatedAt ao chamar SetModified")]
    public void Should_UpdateUpdatedAtProperty_When_CallingSetModified()
    {
        // Arrange
        var invoiceNote = new InvoiceNote();
        var initialUpdatedAt = invoiceNote.UpdatedAt;

        Thread.Sleep(50);

        // Act
        invoiceNote.SetModified();

        // Assert
        Assert.True(invoiceNote.UpdatedAt > initialUpdatedAt);
    }
}