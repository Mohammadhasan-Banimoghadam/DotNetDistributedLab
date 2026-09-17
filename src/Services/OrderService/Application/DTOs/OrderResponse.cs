namespace OrderService.Application.DTOs;

public record OrderResponse(
    Guid Id,
    int CustomerId,
    decimal TotalAmount,
    string Status,
    DateTime CreatedAtUtc);