namespace OrderService.Domain.Entities;

public class Order
{
    public Guid Id { get; private set; }

    public int CustomerId { get; private set; }

    public decimal TotalAmount { get; private set; }

    public OrderStatus Status { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public ICollection<OrderItem> Items { get; private set; } = new List<OrderItem>();

    private Order()
    {
    }

    public Order(int customerId)
    {
        if (customerId <= 0)
            throw new ArgumentException(
                "CustomerId must be greater than zero.",
                nameof(customerId));

        Id = Guid.NewGuid();
        CustomerId = customerId;
        TotalAmount = 0;
        Status = OrderStatus.Pending;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void AddItem(OrderItem item)
    {
        if (item is null)
            throw new ArgumentNullException(nameof(item));

        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException(
                "Items cannot be added to a non-pending order.");

        Items.Add(item);

        RecalculateTotal();
    }

    private void RecalculateTotal()
    {
        TotalAmount = Items.Sum(x => x.GetTotalPrice());
    }

    public void MarkAsPaid()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException(
                "Only pending orders can be marked as paid.");

        if (TotalAmount <= 0)
            throw new InvalidOperationException(
                "Order total amount must be greater than zero.");

        Status = OrderStatus.Paid;
    }

    public void Cancel()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException(
                "Only pending orders can be cancelled.");

        Status = OrderStatus.Cancelled;
    }
}

public enum OrderStatus
{
    Pending = 1,
    Paid = 2,
    Cancelled = 3
}