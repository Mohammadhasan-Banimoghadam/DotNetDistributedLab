namespace OrderService.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; private set; }

    public Guid OrderId { get; private set; }

    public int ProductId { get; private set; }

    public int Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    private OrderItem()
    {
    }

    public OrderItem(
        int productId,
        int quantity,
        decimal unitPrice)
    {
        if (productId <= 0)
            throw new ArgumentException(
                "ProductId must be greater than zero.",
                nameof(productId));

        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.",
                nameof(quantity));

        if (unitPrice <= 0)
            throw new ArgumentException(
                "UnitPrice must be greater than zero.",
                nameof(unitPrice));

        Id = Guid.NewGuid();
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public decimal GetTotalPrice()
    {
        return Quantity * UnitPrice;
    }
}