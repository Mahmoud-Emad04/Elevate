using Elevate.Api.Data;
using Elevate.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Elevate.Api.Repositories;

public class CustomerRepository(OrderManagementDbContext dbContext) : ICustomerRepository
{
    public async Task<Customer> AddAsync(Customer customer, CancellationToken cancellationToken)
    {
        dbContext.Customers.Add(customer);
        await dbContext.SaveChangesAsync(cancellationToken);
        return customer;
    }

    public Task<Customer?> GetAsync(int customerId, CancellationToken cancellationToken)
    {
        return dbContext.Customers.FirstOrDefaultAsync(customer => customer.CustomerId == customerId, cancellationToken);
    }

    public Task<List<Order>> GetOrdersAsync(int customerId, CancellationToken cancellationToken)
    {
        return dbContext.Orders
            .Include(order => order.OrderItems)
            .ThenInclude(item => item.Product)
            .Where(order => order.CustomerId == customerId)
            .ToListAsync(cancellationToken);
    }
}
