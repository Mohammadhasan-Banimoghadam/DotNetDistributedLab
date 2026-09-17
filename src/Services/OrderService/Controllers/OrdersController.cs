using Microsoft.AspNetCore.Mvc;
using OrderService.Application.DTOs;
using OrderService.Application.Services;

namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderApplicationService _orderService;
    private readonly ILogger<OrdersController> _logger; 

    public OrdersController(
        OrderApplicationService orderService,
        ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponse>> Create(
        CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating order for customer {CustomerId}",
            request.CustomerId);

        var result = await _orderService.CreateOrderAsync(
            request,
            cancellationToken);

        return Created(
            $"/api/orders/{result.Id}",
            result);
    }
}


