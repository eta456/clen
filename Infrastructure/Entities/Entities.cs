namespace CleanApi.Infrastructure.Entities;

public interface IBaseEntity
{
    int Id { get; set; }
}

public class CustomerEntity : IBaseEntity
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string EmailAddress { get; set; }
    public ICollection<OrderEntity> Orders { get; set; }
}

public class OrderStatusLookupEntity : IBaseEntity
{
    public int Id { get; set; }
    public string StatusName { get; set; }
}

public class OrderEntity : IBaseEntity
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int StatusId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }

    public CustomerEntity Customer { get; set; }
    public OrderStatusLookupEntity Status { get; set; }
}