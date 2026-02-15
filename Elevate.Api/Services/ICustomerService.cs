using Elevate.Api.Dtos;
using Elevate.Api.Models;

namespace Elevate.Api.Services;

public interface ICustomerService
{
    Task<CustomerResponse> CreateCustomerAsync(CustomerCreateRequest request, CancellationToken cancellationToken);
}
