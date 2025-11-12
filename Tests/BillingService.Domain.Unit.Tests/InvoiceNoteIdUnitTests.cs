using BillingService.Domain.Exceptions;
using BillingService.Domain.ValueObjects;

namespace BillingService.Domain.Unit.Tests;

public class InvoiceNoteIdUnitTests
{
    [Fact(DisplayName = "Deve criar um InvoiceNoteId válido ao chamar NewId")]
    public void Should_CreateValidInvoiceNoteId_When_CallingNewId()
    {
        // Arrange & Act
        var invoiceNoteId = InvoiceNoteId.NewId();

        // Assert
        Assert.NotNull(invoiceNoteId);
        Assert.NotEqual(Guid.Empty, invoiceNoteId.Value);
    }

    [Fact(DisplayName = "Deve criar um InvoiceNoteId válido ao usar o método From")]
    public void Should_CreateValidInvoiceNoteId_When_CallingFrom()
    {
        // Arrange
        var validGuid = Guid.NewGuid();

        // Act
        var invoiceNoteId = InvoiceNoteId.From(validGuid);

        // Assert
        Assert.NotNull(invoiceNoteId);
        Assert.Equal(validGuid, invoiceNoteId.Value);
    }

    [Fact(DisplayName = "Deve lançar InvalidInvoiceNoteIdException quando o valor for um Guid vazio")]
    public void Should_Throw_InvalidInvoiceNoteIdException_When_ValueIsGuidEmpty()
    {
        // Arrange
        var emptyGuid = Guid.Empty;

        // Act & Assert
        Assert.Throws<InvalidInvoiceNoteIdException>(() =>
        {
            InvoiceNoteId id = emptyGuid;
        });
    }

    [Fact(DisplayName = "Deve ser igual ao comparar duas instâncias com o mesmo Guid (Igualdade de Valor)")]
    public void Should_BeEqual_When_ComparingTwoInstancesWithSameGuid()
    {
        // Arrange
        var guid = Guid.NewGuid();
        InvoiceNoteId idOne = guid;
        InvoiceNoteId idTwo = guid;

        // Act & Assert
        Assert.Equal(idOne, idTwo);
        Assert.True(idOne == idTwo);
        Assert.False(idOne != idTwo);
    }

    [Fact(DisplayName = "Não deve ser igual ao comparar dois InvoiceNoteIds diferentes")]
    public void Should_NotBeEqual_When_ComparingTwoDifferentIds()
    {
        // Arrange
        var idOne = InvoiceNoteId.NewId();
        var idTwo = InvoiceNoteId.NewId();

        // Act & Assert
        Assert.NotEqual(idOne, idTwo);
        Assert.False(idOne == idTwo);
        Assert.True(idOne != idTwo);
    }

    [Fact(DisplayName = "Deve converter implicitamente para Guid")]
    public void Should_ImplicitlyConvertToGuid_When_AssignedToGuidVariable()
    {
        // Arrange
        var originalGuid = Guid.NewGuid();
        InvoiceNoteId invoiceNoteId = originalGuid;

        // Act
        Guid resultGuid = invoiceNoteId;

        // Assert
        Assert.Equal(originalGuid, resultGuid);
    }

    [Fact(DisplayName = "Deve converter implicitamente de Guid para InvoiceNoteId")]
    public void Should_ImplicitlyConvertFromGuid_When_AssignedToInvoiceNoteIdVariable()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        InvoiceNoteId invoiceNoteId = guid;

        // Assert
        Assert.NotNull(invoiceNoteId);
        Assert.Equal(guid, invoiceNoteId.Value);
    }

    [Fact(DisplayName = "Deve retornar a representação em string do Guid ao chamar ToString")]
    public void Should_ReturnGuidString_When_CallingToString()
    {
        // Arrange
        var guid = Guid.NewGuid();
        InvoiceNoteId invoiceNoteId = guid;

        // Act
        var resultString = invoiceNoteId.ToString();

        // Assert
        Assert.Equal(guid.ToString(), resultString);
    }
}