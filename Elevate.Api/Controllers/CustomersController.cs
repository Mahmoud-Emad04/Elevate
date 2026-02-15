using Elevate.Api.Dtos;
using Elevate.Api.Mappings;
using Elevate.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Elevate.Api.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController(ICustomerService customerService, IOrderService orderService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CustomerResponse>> CreateCustomer([FromBody] CustomerCreateRequest request, CancellationToken cancellationToken)
    {
        var customer = await customerService.CreateCustomerAsync(request, cancellationToken);
        return CreatedAtAction(nameof(CreateCustomer), customer);
    }

    [HttpGet("{customerId}/orders")]
    public async Task<ActionResult<List<OrderResponse>>> GetCustomerOrders([FromRoute] int customerId, CancellationToken cancellationToken)
    {
        var orders = await orderService.GetOrdersForCustomerAsync(customerId, cancellationToken);
        var response = orders.Select(order => order.ToResponse()).ToList();
        return Ok(response);
    }
}

