using CleanApi.Core.Interfaces;
using CleanApi.Core.Models;

namespace CleanApi.Core.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository; // We keep the specific one for full CRUD
    private readonly ILookupRepository<CustomerModel> _customerLookup;
    private readonly ILookupRepository<StatusModel> _statusLookup;

	// We inject the INTERFACES, never the concrete classes
    public OrderService(
        IOrderRepository orderRepo, 
        ILookupRepository<CustomerModel> customerLookup, 
        ILookupRepository<StatusModel> statusLookup)
    {
        _orderRepository = orderRepo;
        _customerLookup = customerLookup;
        _statusLookup = statusLookup;
    }

    public async Task<OrderModel> CreateOrderAsync(CreateOrderModel createModel)
    {
        // The service just calls the interface method. 
        // The Dependency Injection container handles routing this to the actual database class.
        var customer = await _customerLookup.FindAsync(c => c.Name == createModel.CustomerName);
        if (customer == null) throw new ArgumentException($"Customer '{createModel.CustomerName}' not found.");

        var status = await _statusLookup.FindAsync(s => s.Name == createModel.StatusName);
        if (status == null) throw new ArgumentException($"Status '{createModel.StatusName}' not found.");

        var newOrder = OrderModel.CreateNew(customer.Id, status.Name, createModel.TotalAmount);

        return await _orderRepository.AddAsync(newOrder);
    }

    public async Task UpdateOrderStatusAsync(UpdateOrderStatusModel updateModel)
    {
        var order = await _orderRepository.GetByIdAsync(updateModel.OrderId);
        if (order == null) throw new ArgumentException($"Order {updateModel.OrderId} not found.");

        var newStatus = await _statusRepository.GetByNameAsync(updateModel.NewStatusName);
        if (newStatus == null) throw new ArgumentException($"Status '{updateModel.NewStatusName}' not found.");

        // Apply the business logic
        order.UpdateStatus(newStatus.Name);

        // Save the changes
        await _orderRepository.UpdateAsync(order);
    }

    public async Task DeleteOrderAsync(int orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null) throw new ArgumentException($"Order {orderId} not found.");

        await _orderRepository.DeleteAsync(orderId);
    }
}