using CleanApi.Core.Models;

namespace CleanApi.Core.Interfaces;

public interface ICustomerRepository
{
    Task<CustomerModel> GetByNameAsync(string name);
}

public interface IStatusRepository
{
    Task<StatusModel> GetByNameAsync(string name);
}

// The repository now supports full CRUD operations
public interface IOrderRepository
{
    Task<OrderModel> GetByIdAsync(int id);
    Task<IEnumerable<OrderModel>> GetOrdersByCustomerIdAsync(int customerId);
    Task<OrderModel> AddAsync(OrderModel order);
    Task UpdateAsync(OrderModel order);
    Task DeleteAsync(int id);
}

// The contract for our business orchestration now includes update and delete
public interface IOrderService
{
    Task<OrderModel> CreateOrderAsync(CreateOrderModel model);
    Task UpdateOrderStatusAsync(UpdateOrderStatusModel model);
    Task DeleteOrderAsync(int orderId);
}