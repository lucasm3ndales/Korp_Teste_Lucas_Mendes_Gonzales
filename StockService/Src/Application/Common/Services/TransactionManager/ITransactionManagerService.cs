namespace StockService.Application.Common.Services.TransactionManager;

public interface ITransactionManagerService
{
    Task<IDisposable> BeginTransaction(CancellationToken cancellationToken);
    Task CommitTransaction(CancellationToken cancellationToken);
    Task RollbackTransaction(CancellationToken cancellationToken);
}