namespace StockService.Application.Common.Dtos;

public record ProductDto(
    Guid ProductId,
    string Code,
    string Description,
    int StockBalance,
    DateTimeOffset CreatedAt
);