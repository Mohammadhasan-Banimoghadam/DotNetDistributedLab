namespace PaymentService.Models;

public class CreatePaymentRequest
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
}