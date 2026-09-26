using OrderService.Application.Clients;
using OrderService.Application.DTOs;
using OrderService.Application.Exceptions;
using OrderService.Domain.Entities;
using OrderService.Infrastructure.Persistence;

namespace OrderService.Application.Services;

public class OrderApplicationService
{
    private readonly CustomerClient _customerClient;
    private readonly ProductClient _productClient;
    private readonly PaymentClient _paymentClient;
    private readonly ILogger<OrderApplicationService> _logger;
    private readonly OrderDbContext _db;

    public OrderApplicationService(
        CustomerClient customerClient,
        ProductClient productClient,
        PaymentClient paymentClient,
        OrderDbContext db,
        ILogger<OrderApplicationService> logger)
    {
        _customerClient = customerClient;
        _productClient = productClient;
        _paymentClient = paymentClient;
        _db = db;
        _logger = logger;
    }

    public async Task<OrderResponse> CreateOrderAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Creating order for CustomerId: {CustomerId}",
            request.CustomerId);

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

        var order = new Order(request.CustomerId);

        foreach (var itemRequest in request.Items)
        {
            var product = await _productClient.GetProductAsync(
                itemRequest.ProductId,
                cancellationToken);

            if (product is null)
            {
                throw new Exception(
                    $"Product not found. ProductId: {itemRequest.ProductId}");
            }

            if (!product.IsActive)
            {
                throw new Exception(
                    $"Product is inactive. ProductId: {itemRequest.ProductId}");
            }

            if (itemRequest.Quantity > product.Stock)
            {
                throw new Exception(
                    $"Insufficient stock for ProductId: {itemRequest.ProductId}");
            }

            var orderItem = new OrderItem(
                product.Id,
                itemRequest.Quantity,
                product.Price);

            order.AddItem(orderItem);
        }

        _db.Orders.Add(order);

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Order created successfully. OrderId: {OrderId}, TotalAmount: {TotalAmount}",
            order.Id,
            order.TotalAmount);

        var payment = await _paymentClient.CreatePaymentAsync(
            order.Id,
            order.TotalAmount,
            cancellationToken);

        if (payment is null)
        {
            _logger.LogError(
                "Payment failed. Cancelling order. OrderId: {OrderId}",
                order.Id);

            order.Cancel();

            await _db.SaveChangesAsync(cancellationToken);

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

    public async Task<OrderResponse> CancelOrderAsync(
    Guid orderId,
    CancellationToken cancellationToken = default)
    {
        var order = await _db.Orders.FindAsync(
            new object[] { orderId },
            cancellationToken);

        if (order is null)
        {
            throw new KeyNotFoundException(
                $"Order not found. OrderId: {orderId}");
        }

        order.Cancel();

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Order cancelled successfully. OrderId: {OrderId}",
            order.Id);

        return new OrderResponse(
            order.Id,
            order.CustomerId,
            order.TotalAmount,
            order.Status.ToString(),
            order.CreatedAtUtc);
    }
}