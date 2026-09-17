namespace OrderService.Application.Clients;

public class PaymentClient
{
    private readonly HttpClient _httpClient;

    public PaymentClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PaymentResponse?> CreatePaymentAsync(
        Guid orderId,
        decimal amount,
        CancellationToken cancellationToken = default)
    {
        var request = new
        {
            OrderId = orderId,
            Amount = amount
        };

        var response = await _httpClient.PostAsJsonAsync(
            "api/Payments",
            request,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<PaymentResponse>(
            cancellationToken: cancellationToken);
    }
}

public record PaymentResponse(
    int PaymentId,
    Guid OrderId,
    decimal Amount,
    string Status);