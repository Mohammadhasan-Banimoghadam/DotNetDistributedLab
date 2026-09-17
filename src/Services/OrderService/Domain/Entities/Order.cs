namespace OrderService.Domain.Entities;

public class Order
{
    public Guid Id { get; private set; }

    public int CustomerId { get; private set; }

    public decimal TotalAmount { get; private set; }

    public OrderStatus Status { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private Order()
    {
    }

    public Order(
        int customerId,
        decimal totalAmount)
    {
        if (customerId == 0)
            throw new ArgumentException(
                "CustomerId cannot be empty.",
                nameof(customerId));

        if (totalAmount <= 0)
            throw new ArgumentException(
                "Total amount must be greater than zero.",
                nameof(totalAmount));

        Id = Guid.NewGuid();
        CustomerId = customerId;
        TotalAmount = totalAmount;
        Status = OrderStatus.Pending;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void MarkAsPaid()
    {
        Status = OrderStatus.Paid;
    }

    public void Cancel()
    {
        Status = OrderStatus.Cancelled;
    }
}

public enum OrderStatus
{
    Pending = 1,
    Paid = 2,
    Cancelled = 3
}