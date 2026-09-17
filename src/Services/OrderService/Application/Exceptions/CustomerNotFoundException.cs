namespace OrderService.Application.Exceptions;

public class CustomerNotFoundException : Exception
{
    public int CustomerId { get; }

    public CustomerNotFoundException(int customerId)
        : base($"Customer with id '{customerId}' was not found.")
    {
        CustomerId = customerId;
    }
}