using Mapster;
using Microsoft.EntityFrameworkCore;
using CleanApi.Core.Interfaces;
using CleanApi.Core.Models;
using CleanApi.Infrastructure.Data;
using CleanApi.Infrastructure.Entities;

namespace CleanApi.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;
    public CustomerRepository(AppDbContext context) => _context = context;

    public async Task<CustomerModel> GetByNameAsync(string name) => 
        await _context.Customers
            .AsNoTracking()
            .Where(c => c.FullName == name)
            .ProjectToType<CustomerModel>()
            .FirstOrDefaultAsync();
}

public class StatusRepository : IStatusRepository
{
    private readonly AppDbContext _context;
    public StatusRepository(AppDbContext context) => _context = context;

    public async Task<StatusModel> GetByNameAsync(string name) => 
        await _context.OrderStatusLookups
            .AsNoTracking()
            .Where(s => s.StatusName == name)
            .ProjectToType<StatusModel>()
            .FirstOrDefaultAsync();
}

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;
    
    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<OrderModel> GetByIdAsync(int id)
    {
        return await _context.Orders
            .AsNoTracking()
            .Where(o => o.Id == id)
            .ProjectToType<OrderModel>()
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<OrderModel>> GetOrdersByCustomerIdAsync(int customerId)
    {
        return await _context.Orders
            .AsNoTracking()
            .Where(o => o.CustomerId == customerId)
            .ProjectToType<OrderModel>()
            .ToListAsync();
    }

    public async Task<OrderModel> AddAsync(OrderModel order)
    {
        var entity = order.Adapt<OrderEntity>();
        
        await _context.Orders.AddAsync(entity);
        await _context.SaveChangesAsync();
        
        return entity.Adapt<OrderModel>();
    }

    public async Task UpdateAsync(OrderModel order)
    {
        var entity = await _context.Orders.FindAsync(order.Id);
        
        if (entity != null)
        {
            order.Adapt(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Orders.FindAsync(id);
        
        if (entity != null)
        {
            _context.Orders.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}