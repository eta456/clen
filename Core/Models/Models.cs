namespace CleanApi.Core.Models;

// A simple interface to ensure all models have a standard identifier
public interface IBaseModel
{
    int Id { get; set; }
}

// Anemic Model: A simple data container with no strict behaviour rules
public class CustomerModel : IBaseModel
{
    public int Id { get; set; }
    public string Name { get; set; } 
    public string Email { get; set; }
    
    // IEnumerable protects the collection from being manipulated outside of intended workflows
    public IEnumerable<OrderModel> Orders { get; set; } = new List<OrderModel>();
}

// Anemic Model: Used purely for reference data
public class StatusModel : IBaseModel
{
    public int Id { get; set; }
    public string Name { get; set; }
}

// Rich Domain Model: State is protected and behaviour is strictly controlled
public class OrderModel : IBaseModel
{
    public int Id { get; set; } 
    public int CustomerId { get; private set; }
    public string StatusName { get; private set; } 
    public DateTime OrderDate { get; private set; }
    public decimal TotalAmount { get; private set; }

    // A parameterless constructor is required by Mapster to materialise this from the database
    private OrderModel() { }

    // Factory Method: This is the only way to create a valid order, enforcing our business rules
    public static OrderModel CreateNew(int customerId, string statusName, decimal initialAmount)
    {
        if (initialAmount <= 0) 
            throw new ArgumentException("Initial amount must be greater than zero.");

        return new OrderModel
        {
            CustomerId = customerId,
            StatusName = statusName,
            TotalAmount = initialAmount,
            OrderDate = DateTime.UtcNow
        };
    }

    // Behaviour Method: The only way to update an order's status
    public void UpdateStatus(string newStatus)
    {
        if (string.IsNullOrWhiteSpace(newStatus))
            throw new ArgumentException("Status cannot be empty.");
            
        StatusName = newStatus;
    }
}

// Data models used to pass data from the API to the Service layer
public class CreateOrderModel
{
    public string CustomerName { get; set; }
    public string StatusName { get; set; }
    public decimal TotalAmount { get; set; }
}

public class UpdateOrderStatusModel
{
    public int OrderId { get; set; }
    public string NewStatusName { get; set; }
}