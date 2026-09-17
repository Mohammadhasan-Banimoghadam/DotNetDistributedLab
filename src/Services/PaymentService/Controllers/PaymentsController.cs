using Microsoft.AspNetCore.Mvc;
using PaymentService.Models;

namespace PaymentService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    [HttpPost]
    public IActionResult CreatePayment(CreatePaymentRequest request)
    {
        if (request.Amount <= 0)
        {
            return BadRequest(new
            {
                message = "Amount must be greater than zero."
            });
        }

        var payment = new PaymentResponse
        {
            PaymentId = Random.Shared.Next(1000, 9999),
            OrderId = request.OrderId,
            Amount = request.Amount,
            Status = "Paid"
        };

        return Ok(payment);
    }
}