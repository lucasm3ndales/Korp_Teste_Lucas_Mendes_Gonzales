using System.ComponentModel.DataAnnotations;
using StockService.Domain.Exceptions;

namespace StockService.Domain.Entities;

public class Product
{
    public long Id { get; private set; }

    public Guid ProductId { get; private set; }

    public string Code { get; private set; }

    public string Description { get; private set; }

    public int StockBalance { get; private set; }
    
    public byte[] RowVersion { get; private set; } = null!;
    
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
        
        ProductId = Guid.NewGuid();
        Code = code;
        Description = description;
        StockBalance = initialStockBalance;
    }
    
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
}