using Elevate.Api.Dtos;
using Elevate.Api.Models;
using Elevate.Api.Repositories;

namespace Elevate.Api.Services;

public class CustomerService(ICustomerRepository customerRepository) : ICustomerService
{
    public async Task<CustomerResponse> CreateCustomerAsync(CustomerCreateRequest request, CancellationToken cancellationToken)
    {
        var customer = new Customer
        {
            Name = request.Name,
            Email = request.Email
        };

        await customerRepository.AddAsync(customer, cancellationToken);
        return new CustomerResponse(customer.CustomerId, customer.Name, customer.Email);
    }
}
