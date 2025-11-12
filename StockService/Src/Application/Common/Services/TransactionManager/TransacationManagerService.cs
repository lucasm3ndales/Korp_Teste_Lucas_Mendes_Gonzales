using Microsoft.EntityFrameworkCore.Storage;
using StockService.Infra.Data;

namespace StockService.Application.Common.Services.TransactionManager;

public class TransacationManagerService(StockDbContext dbContext): ITransactionManagerService
{
    private IDbContextTransaction _currentTransaction;

    public async Task<IDisposable> BeginTransaction(CancellationToken cancellationToken)
    {
        if (_currentTransaction != null)
            throw new InvalidOperationException("Uma transação já está em andamento.");

        _currentTransaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        return _currentTransaction;
    }

    public async Task CommitTransaction(CancellationToken cancellationToken)
    {
        if (_currentTransaction == null)
            throw new InvalidOperationException("Nenhuma transação para commitar.");
            
        await dbContext.SaveChangesAsync(cancellationToken);
        await _currentTransaction.CommitAsync(cancellationToken);
    }

    public async Task RollbackTransaction(CancellationToken cancellationToken)
    {
        if (_currentTransaction == null)
            throw new InvalidOperationException("Nenhuma transação para rollback.");

        await _currentTransaction.RollbackAsync(cancellationToken);
    }
}