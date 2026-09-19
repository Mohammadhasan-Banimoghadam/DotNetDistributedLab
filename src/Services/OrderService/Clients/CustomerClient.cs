using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using OrderService.Application.DTOs;
using OrderService.Application.Exceptions;
using Polly.CircuitBreaker;

namespace OrderService.Application.Clients;

public class CustomerClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CustomerClient> _logger;

    public CustomerClient(HttpClient httpClient, ILogger<CustomerClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<CustomerResponse?> GetCustomerAsync(
        int customerId,
        CancellationToken cancellationToken = default)
    {


        _logger.LogInformation(
            "Requesting CustomerService for CustomerId: {CustomerId}",
            customerId);

        try
        {
            var response = await _httpClient.GetAsync(
                $"api/Customers/{customerId}",
                cancellationToken);

            _logger.LogInformation(
                "CustomerService returned StatusCode: {StatusCode}",
                response.StatusCode);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<CustomerResponse>(
                    cancellationToken);
        }
        catch (BrokenCircuitException ex)
        {
            _logger.LogWarning(
                ex,
                "Circuit breaker is open for CustomerService.");

            throw new ServiceUnavailableException(
                "CustomerService",
                ex);
        }
        catch (OperationCanceledException)
            when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(
                "CustomerService request timed out.");

            throw new ServiceUnavailableException(
                "CustomerService",
                new TimeoutException(
                    "CustomerService request timed out."));
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(
                ex,
                "CustomerService is unavailable.");

            throw new ServiceUnavailableException(
                "CustomerService",
                ex);
        }
    }
}


