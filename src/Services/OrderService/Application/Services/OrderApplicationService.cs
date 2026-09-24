using OrderService.Application.Clients;
using OrderService.Application.DTOs;
using OrderService.Application.Exceptions;
using OrderService.Domain.Entities;
using OrderService.Infrastructure.Persistence;

namespace OrderService.Application.Services;

public class OrderApplicationService
{
    private readonly CustomerClient _customerClient;
    private readonly PaymentClient _paymentClient;
    private readonly ILogger<OrderApplicationService> _logger;
    private readonly OrderDbContext _db;

    public OrderApplicationService(
        CustomerClient customerClient,
        PaymentClient paymentClient,
        OrderDbContext db,
        ILogger<OrderApplicationService> logger)
    {
        _customerClient = customerClient;
        _paymentClient = paymentClient;
        _logger = logger;
        _db = db;
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

        _db.Orders.Add(order);

        await _db.SaveChangesAsync(cancellationToken);

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

        order.MarkAsPaid();

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Order marked as paid. OrderId: {OrderId}",
            order.Id);

        return new OrderResponse(
            order.Id,
            order.CustomerId,
            order.TotalAmount,
            order.Status.ToString(),
            order.CreatedAtUtc);
    }
}
