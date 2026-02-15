using Elevate.Api.Dtos;
using Elevate.Api.Mappings;
using Elevate.Api.Models;
using Elevate.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elevate.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<OrderResponse>> CreateOrder([FromBody] OrderCreateRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await orderService.CreateOrderAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetOrder), new { orderId = order.OrderId }, order.ToResponse());
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpGet("{orderId:int}")]
    public async Task<ActionResult<OrderResponse>> GetOrder([FromRoute] int orderId, CancellationToken cancellationToken)
    {
        var order = await orderService.GetOrderAsync(orderId, cancellationToken);
        if (order is null)
        {
            return NotFound();
        }

        return Ok(order.ToResponse());
    }

    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpGet]
    public async Task<ActionResult<List<OrderResponse>>> GetOrders(CancellationToken cancellationToken)
    {
        var orders = await orderService.GetOrdersAsync(cancellationToken);
        var response = orders.Select(order => order.ToResponse()).ToList();
        return Ok(response);
    }

    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpPut("{orderId}/status")]
    public async Task<ActionResult<OrderResponse>> UpdateStatus([FromRoute] int orderId, [FromBody] UpdateOrderStatusRequest request, CancellationToken cancellationToken)
    {
        var order = await orderService.UpdateStatusAsync(orderId, request.Status, cancellationToken);
        if (order is null)
        {
            return NotFound();
        }

        return Ok(order.ToResponse());
    }
}
