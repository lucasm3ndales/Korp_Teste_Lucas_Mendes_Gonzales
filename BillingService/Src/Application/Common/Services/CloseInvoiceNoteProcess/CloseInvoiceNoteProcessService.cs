using BillingService.Application.Common.Repositories;
using BillingService.Application.UseCases.CloseInvoiceNote;
using MediatR;

namespace BillingService.Application.Common.Services.CloseInvoiceNoteProcess;

public class CloseInvoiceNoteProcessService(
    IServiceProvider serviceProvider,
    ILogger<CloseInvoiceNoteProcessService> logger)
    : BackgroundService
{
    private readonly TimeSpan _invoiceNoteProcessTimeout = TimeSpan.FromMinutes(15);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

            using var scope = serviceProvider.CreateScope();
            await ProcessStuckInvoiceNotes(scope, stoppingToken);
        }
    }

    private async Task ProcessStuckInvoiceNotes(IServiceScope scope, CancellationToken cancellationToken)
    {
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        
        var invoiceNoteRepository = scope.ServiceProvider.GetRequiredService<IInvoiceNoteRepository>();
        
        var staleTime = DateTimeOffset.UtcNow.Subtract(_invoiceNoteProcessTimeout);
        
        var stuckInvoiceNotes = await invoiceNoteRepository.GetProcessingInvoiceNotesOlderThan(
            staleTime, 
            cancellationToken
        );
        
        foreach (var i in stuckInvoiceNotes)
        {
            var command = new CloseInvoiceNoteCommand
            {
                Id = i.Id,
                Xmin = i.Xmin,
                IsSyncProcess = true
            };
            
            try
            {
                logger.LogInformation("Tentando finalizar NF presa: {Id}", i.Id);
                await mediator.Send(command, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Falha na reconciliação da NF {Id}. Tentativa abortada.", i.Id);
            }
        }
    }
}