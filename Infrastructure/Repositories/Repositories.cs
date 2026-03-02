	using Mapster;
using Microsoft.EntityFrameworkCore;
using CleanApi.Core.Interfaces;
using CleanApi.Core.Models;
using CleanApi.Infrastructure.Data;
using CleanApi.Infrastructure.Entities;

namespace CleanApi.Infrastructure.Repositories;

public class LookupRepository<TEntity, TModel> : ILookupRepository<TModel> 
    where TEntity : class 
    where TModel : class
{
    private readonly AppDbContext _context;

    public LookupRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TModel> FindAsync(Expression<Func<TModel, bool>> predicate)
    {
        return await _context.Set<TEntity>()
            .AsNoTracking()
            // Mapster creates an IQueryable of TModel, allowing EF Core to map the SQL directly
            .ProjectToType<TModel>()
            // We apply the expression to the translated query
            .Where(predicate)
            .FirstOrDefaultAsync();
    }
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