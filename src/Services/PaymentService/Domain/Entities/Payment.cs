namespace PaymentService.Domain.Entities;

public class Payment
{
    public int Id { get; private set; }

    public Guid OrderId { get; private set; }

    public decimal Amount { get; private set; }

    public PaymentStatus Status { get; private set; }

    private Payment()
    {
    }

    public Payment(
        Guid orderId,
        decimal amount)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentException(
                "OrderId cannot be empty.",
                nameof(orderId));

        if (amount <= 0)
            throw new ArgumentException(
                "Amount must be greater than zero.",
                nameof(amount));

        OrderId = orderId;
        Amount = amount;
        Status = PaymentStatus.Paid;
    }
}

public enum PaymentStatus
{
    Pending = 1,
    Paid = 2,
    Failed = 3
}