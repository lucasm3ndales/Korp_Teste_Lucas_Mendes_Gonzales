using StockService.Domain.Exceptions;
using StockService.Domain.ValueObjects;

namespace StockService.Domain.Tests;

public class ProductIdTests
{
    [Fact]
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

    [Fact]
    public void Should_CreateValidProductId_When_CallingNewId()
    {
        // Arrange
        // No arrangement needed

        // Act
        var productId = ProductId.NewId();

        // Assert
        Assert.NotNull(productId);
        Assert.NotEqual(Guid.Empty, productId.Value);
    }

    [Fact]
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

    [Fact]
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
    
    [Fact]
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
}