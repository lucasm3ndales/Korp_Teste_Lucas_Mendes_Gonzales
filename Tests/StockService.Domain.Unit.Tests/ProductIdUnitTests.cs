using StockService.Domain.Exceptions;
using StockService.Domain.ValueObjects;

namespace StockService.Domain.Unit.Tests;

public class ProductIdTests
{
    [Fact(DisplayName = "Deve lançar InvalidProductIdException quando o valor for um Guid vazio")]
    public void Should_Throw_InvalidProductIdException_When_ValueIsGuidEmpty()
    {
        // Arrange
        var emptyGuid = Guid.Empty;

        // Act & Assert
        Assert.Throws<InvalidProductIdException>(() =>
        {
            ProductId id = emptyGuid;
        });
    }

    [Fact(DisplayName = "Deve criar um ProductId válido ao chamar NewId")]
    public void Should_CreateValidProductId_When_CallingNewId()
    {
        // Act
        var productId = ProductId.NewId();

        // Assert
        Assert.NotNull(productId);
        Assert.NotEqual(Guid.Empty, productId.Value);
    }

    [Fact(DisplayName = "Deve ser igual ao comparar duas instâncias com o mesmo Guid")]
    public void Should_BeEqual_When_ComparingTwoInstancesWithSameGuid()
    {
        // Arrange
        var guid = Guid.NewGuid();
        ProductId idOne = guid;
        ProductId idTwo = guid;

        // Act
        var areEqual = idOne == idTwo;

        // Assert
        Assert.Equal(idOne, idTwo);
        Assert.True(areEqual);
    }

    [Fact(DisplayName = "Não deve ser igual ao comparar dois ProductIds diferentes")]
    public void Should_NotBeEqual_When_ComparingTwoDifferentIds()
    {
        // Arrange
        var idOne = ProductId.NewId();
        var idTwo = ProductId.NewId();

        // Act
        var areNotEqual = idOne != idTwo;

        // Assert
        Assert.NotEqual(idOne, idTwo);
        Assert.True(areNotEqual);
    }

    [Fact(DisplayName = "Deve converter implicitamente para Guid ao ser atribuído a uma variável Guid")]
    public void Should_ImplicitlyConvertToGuid_When_AssignedToGuidVariable()
    {
        // Arrange
        var originalGuid = Guid.NewGuid();
        ProductId productId = originalGuid;

        // Act
        Guid resultGuid = productId;

        // Assert
        Assert.Equal(originalGuid, resultGuid);
    }
    
    [Fact(DisplayName = "Deve converter implicitamente de Guid para ProductId")]
    public void Should_ImplicitlyConvertFromGuid_When_AssignedToProductIdVariable()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        ProductId productId = guid;

        // Assert
        Assert.NotNull(productId);
        Assert.Equal(guid, productId.Value);
    }
    
    [Fact(DisplayName = "Deve criar um ProductId a partir de um Guid usando o método From")]
    public void Should_CreateProductId_When_UsingFromMethod()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var productId = ProductId.From(guid);

        // Assert
        Assert.NotNull(productId);
        Assert.Equal(guid, productId.Value);
    }
}