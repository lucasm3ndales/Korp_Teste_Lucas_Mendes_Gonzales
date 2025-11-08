namespace StockService.Application.Common.Dtos;

public record ProductDto(
    Guid Id,
    string Code,
    string Description,
    int StockBalance,
    DateTimeOffset CreatedAt
);