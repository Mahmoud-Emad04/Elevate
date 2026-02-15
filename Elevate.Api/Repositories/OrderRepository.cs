using Elevate.Api.Data;
using Elevate.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Elevate.Api.Repositories;

public class OrderRepository(OrderManagementDbContext dbContext) : IOrderRepository
{
    public async Task<Order> AddAsync(Order order, CancellationToken cancellationToken)
    {
        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync(cancellationToken);
        return order;
    }

    public Task<Order?> GetAsync(int orderId, CancellationToken cancellationToken)
    {
        return dbContext.Orders
            .Include(order => order.Customer)
            .Include(order => order.OrderItems)
            .ThenInclude(item => item.Product)
            .Include(order => order.Invoice)
            .FirstOrDefaultAsync(order => order.OrderId == orderId, cancellationToken);
    }

    public Task<List<Order>> GetAllAsync(CancellationToken cancellationToken)
    {
        return dbContext.Orders
            .Include(order => order.Customer)
            .Include(order => order.OrderItems)
            .ThenInclude(item => item.Product)
            .Include(order => order.Invoice)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Order>> GetByCustomerAsync(int customerId, CancellationToken cancellationToken)
    {
        return dbContext.Orders
            .Include(order => order.Customer)
            .Include(order => order.OrderItems)
            .ThenInclude(item => item.Product)
            .Where(order => order.CustomerId == customerId)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(Order order, CancellationToken cancellationToken)
    {
        dbContext.Orders.Update(order);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
