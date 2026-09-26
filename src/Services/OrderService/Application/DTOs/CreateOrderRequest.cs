namespace OrderService.Application.DTOs;

public record CreateOrderRequest(
    int CustomerId,
    List<CreateOrderItemRequest> Items);

public record CreateOrderItemRequest(
    int ProductId,
    int Quantity);