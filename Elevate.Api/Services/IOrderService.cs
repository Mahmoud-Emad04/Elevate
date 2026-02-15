using Elevate.Api.Dtos;
using Elevate.Api.Models;

namespace Elevate.Api.Services;

public interface IOrderService
{
    Task<Order> CreateOrderAsync(OrderCreateRequest request, CancellationToken cancellationToken);
    Task<Order?> GetOrderAsync(int orderId, CancellationToken cancellationToken);
    Task<List<Order>> GetOrdersAsync(CancellationToken cancellationToken);
    Task<List<Order>> GetOrdersForCustomerAsync(int customerId, CancellationToken cancellationToken);
    Task<Order?> UpdateStatusAsync(int orderId, OrderStatus status, CancellationToken cancellationToken);
}
