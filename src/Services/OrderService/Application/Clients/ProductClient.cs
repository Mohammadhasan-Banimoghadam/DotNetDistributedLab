using System.Net.Http.Json;
using OrderService.Application.DTOs;

namespace OrderService.Application.Clients;

public class ProductClient
{
    private readonly HttpClient _httpClient;

    public ProductClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ProductResponse?> GetProductAsync(
        int productId,
        CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<ProductResponse>(
            $"api/Products/{productId}",
            cancellationToken);
    }
}