namespace OrderService.Application.Exceptions;

public class ServiceUnavailableException : Exception
{
    public ServiceUnavailableException(string serviceName, Exception innerException)
        : base($"{serviceName} is currently unavailable.", innerException)
    {
    }
}
