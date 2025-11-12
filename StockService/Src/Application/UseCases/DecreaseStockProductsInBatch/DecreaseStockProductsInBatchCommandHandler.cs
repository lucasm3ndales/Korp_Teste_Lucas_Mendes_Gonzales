using MediatR;
using Microsoft.EntityFrameworkCore;
using StockService.Application.Common.Dtos;
using StockService.Application.Common.Exceptions;
using StockService.Application.Common.Repositories;
using StockService.Application.Common.Services.TransactionManager;
using StockService.Domain.Entities;
using StockService.Domain.Exceptions;

namespace StockService.Application.UseCases.DecreaseStockProductsInBatch;

public class DecreaseStockProductsInBatchCommandHandler(
    IProductRepository productRepository,
    IIdempotencyKeyRepository idempotencyKeyRepository,
    ITransactionManagerService transactionManagerService
) : IRequestHandler<DecreaseStockProductsInBatchCommand, ApiResultDto<bool>>
{
    public async Task<ApiResultDto<bool>> Handle(DecreaseStockProductsInBatchCommand request,
        CancellationToken cancellationToken)
    {
        using var transaction = await transactionManagerService.BeginTransaction(cancellationToken);

        try
        {
            if (await idempotencyKeyRepository.Exists(request.InvoiceNoteId, cancellationToken))
            {
                return ApiResultDto<bool>.Success("Operação já processada.", true);
            }

            var items = request
                .Items
                .GroupBy(i => i.Id)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalQuantityUsed = g.Sum(i => i.QuantityUsed),
                    Xmin = g.Select(i => i.Xmin).Min()
                })
                .ToList();

            var productIds = items.Select(i => i.ProductId).ToList();
            var products = await productRepository.GetByIds(productIds, cancellationToken);
            var productDic = products.ToDictionary(p => p.Id);

            foreach (var i in items)
            {
                if (!productDic.TryGetValue(i.ProductId, out var p))
                    throw new ProductNotExistsException();

                if (p.Xmin != i.Xmin)
                {
                    throw new StockConcurrencyException();
                }

                if (p.StockBalance < i.TotalQuantityUsed)
                    throw new InsufficientStockBalanceException(p.Code, p.StockBalance, i.TotalQuantityUsed);
            }

            foreach (var i in items)
            {
                var p = productDic[i.ProductId];

                p.DecreaseStockBalance(i.TotalQuantityUsed);
            }

            await productRepository.SaveChanges(cancellationToken);

            await CreateAndSaveIdempotencyKey(request.InvoiceNoteId, cancellationToken);

            await transactionManagerService.CommitTransaction(cancellationToken);

            return ApiResultDto<bool>.Success("Estoque atualizado com sucesso para todos os itens!", true);
        }
        catch (Exception ex)
        {
            await transactionManagerService.RollbackTransaction(cancellationToken);
            
            if (ex is DbUpdateConcurrencyException)
                throw new StockConcurrencyException();

            throw;
        }
    }

    private async Task CreateAndSaveIdempotencyKey(Guid key, CancellationToken cancellationToken)
    {
        var idempotencyEntry = new IdempotencyKey(key);
        await idempotencyKeyRepository.Add(idempotencyEntry, cancellationToken);
        await idempotencyKeyRepository.SaveChanges(cancellationToken);
    }
}