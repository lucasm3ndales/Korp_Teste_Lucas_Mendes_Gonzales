namespace BillingService.Application.Common.Exceptions;

public class ServiceUnavailableException: ApplicationException
{
    public ServiceUnavailableException(string serviceName, Exception innerException) 
        : base($"Falha de comunicação: O serviço '{serviceName}' está indisponível.", innerException)
    {
    }
}