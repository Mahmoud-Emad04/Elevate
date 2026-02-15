using Elevate.Api.Models;

namespace Elevate.Api.Repositories;

public interface ICustomerRepository
{
    Task<Customer> AddAsync(Customer customer, CancellationToken cancellationToken);
    Task<Customer?> GetAsync(int customerId, CancellationToken cancellationToken);
    Task<List<Order>> GetOrdersAsync(int customerId, CancellationToken cancellationToken);
}
