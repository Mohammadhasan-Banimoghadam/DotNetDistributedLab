namespace PaymentService.Models;

public class PaymentResponse
{
    public int PaymentId { get; set; }
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
}