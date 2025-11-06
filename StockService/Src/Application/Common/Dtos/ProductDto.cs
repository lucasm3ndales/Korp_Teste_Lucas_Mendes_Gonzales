using StockService.Domain.ValueObjects;

namespace StockService.Application.Common.Dtos;

public record ProductDto(
    ProductId Id,
    string Code,
    string Description,
    int StockBalance,
    DateTimeOffset CreatedAt
);