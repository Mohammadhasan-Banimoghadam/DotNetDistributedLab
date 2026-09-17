using OrderService.Application.Clients;
using OrderService.Application.DTOs;
using OrderService.Application.Exceptions;
using OrderService.Domain.Entities;

namespace OrderService.Application.Services;

public class OrderApplicationService
{
    private readonly CustomerClient _customerClient;
    private readonly PaymentClient _paymentClient;
    private readonly ILogger<OrderApplicationService> _logger;

    public OrderApplicationService(
        CustomerClient customerClient,
        PaymentClient paymentClient,
        ILogger<OrderApplicationService> logger)
    {
        _customerClient = customerClient;
        _paymentClient = paymentClient;
        _logger = logger;
    }

    public async Task<OrderResponse> CreateOrderAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Creating order for CustomerId: {CustomerId}, TotalAmount: {TotalAmount}",
            request.CustomerId,
            request.TotalAmount);

        var customer = await _customerClient.GetCustomerAsync(
            request.CustomerId,
            cancellationToken);

        if (customer is null)
        {
            _logger.LogWarning(
                "Customer not found. CustomerId: {CustomerId}",
                request.CustomerId);

            throw new CustomerNotFoundException(
                request.CustomerId);
        }

        var order = new Order(
            request.CustomerId,
            request.TotalAmount);

        _logger.LogInformation(
            "Order created successfully. OrderId: {OrderId}",
            order.Id);

        var payment = await _paymentClient.CreatePaymentAsync(
                order.Id,
                order.TotalAmount,
                cancellationToken);

        if (payment is null)
        {
            _logger.LogError(
                "Payment failed. OrderId: {OrderId}",
                order.Id);

            throw new Exception(
                $"Payment failed for OrderId: {order.Id}");
        }

        return new OrderResponse(
            order.Id,
            order.CustomerId,
            order.TotalAmount,
            order.Status.ToString(),
            order.CreatedAtUtc);
    }
}
