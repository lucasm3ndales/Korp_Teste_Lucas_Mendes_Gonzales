using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StockService.Domain.Exceptions;
using StockService.Domain.ValueObjects;

namespace StockService.Domain.Entities;

public class Product: IEquatable<Product>
{
    public ProductId Id { get; private set; }

    public string Code { get; private set; }

    public string Description { get; private set; }

    public int StockBalance { get; private set; }
    
    public uint Xmin { get; private set; }
    
    public DateTimeOffset CreatedAt { get; private set; }
    
    protected Product()
    {
    }

    public Product(string code, string description, int initialStockBalance)
    {
        if (string.IsNullOrWhiteSpace(code) || code.Length > 20)
            throw new InvalidProductCodeException(code);
        
        if (string.IsNullOrWhiteSpace(description) || description.Length > 255)
            throw new InvalidProductDescriptionException();

        if (initialStockBalance < 0)
            throw new InvalidProductStockBalanceException(initialStockBalance);
        
        Id = ProductId.NewId();
        Code = code;
        Description = description;
        StockBalance = initialStockBalance;
        CreatedAt =  DateTimeOffset.UtcNow;
    }
    
    public void SetXmin(uint xmin) => Xmin = xmin;
    
    public void DecreaseStockBalance(int quantityUsed)
    {
        if (quantityUsed <= 0)
        {
            throw new InvalidStockBalanceQuantityException(quantityUsed, "A Quantidade a ser debitada deve ser positiva.");
        }

        if (StockBalance < quantityUsed)
        {
            throw new InsufficientStockBalanceException(Code, StockBalance, quantityUsed);
        }
        
        StockBalance -= quantityUsed;
    }

    public bool Equals(Product? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;
        
        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Product);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(Product? left, Product? right)
    {
        if (left is null)
            return right is null;
        
        return left.Equals(right);
    }

    public static bool operator !=(Product? left, Product? right)
    {
        return !(left == right);
    }
}