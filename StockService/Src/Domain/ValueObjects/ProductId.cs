using StockService.Domain.Exceptions;

namespace StockService.Domain.ValueObjects;

public record ProductId
{
    public Guid Value { get; }
    
    private ProductId(Guid value)
    {
        if (value == Guid.Empty)
            throw new InvalidProductIdException();
        
        Value = value;
    }
    
    public static ProductId NewId() => new(Guid.NewGuid());
    
    public static implicit operator Guid(ProductId id) => id.Value;
    
    public static implicit operator ProductId(Guid value) => new(value);
    
    public override string ToString() => Value.ToString();
};