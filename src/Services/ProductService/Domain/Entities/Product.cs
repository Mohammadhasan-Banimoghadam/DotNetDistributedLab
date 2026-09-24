namespace ProductService.Domain.Entities;

public class Product
{
    public int Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public decimal Price { get; private set; }

    public int Stock { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private Product()
    {
    }

    public Product(
        string name,
        string description,
        decimal price,
        int stock)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Product name cannot be empty.",
                nameof(name));

        if (price <= 0)
            throw new ArgumentException(
                "Product price must be greater than zero.",
                nameof(price));

        if (stock < 0)
            throw new ArgumentException(
                "Product stock cannot be negative.",
                nameof(stock));

        Name = name;
        Description = description;
        Price = price;
        Stock = stock;
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void Update(
    string name,
    string description,
    decimal price,
    int stock)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Product name cannot be empty.",
                nameof(name));

        if (price <= 0)
            throw new ArgumentException(
                "Product price must be greater than zero.",
                nameof(price));

        if (stock < 0)
            throw new ArgumentException(
                "Product stock cannot be negative.",
                nameof(stock));

        Name = name;
        Description = description;
        Price = price;
        Stock = stock;
    }
}