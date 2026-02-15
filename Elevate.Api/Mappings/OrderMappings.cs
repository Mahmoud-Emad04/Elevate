using Elevate.Api.Dtos;
using Elevate.Api.Models;

namespace Elevate.Api.Mappings;

public static class OrderMappings
{
    public static OrderResponse ToResponse(this Order order)
    {
        var items = order.OrderItems.Select(item => new OrderItemResponse(
            item.ProductId,
            item.Product?.Name ?? string.Empty,
            item.Quantity,
            item.UnitPrice,
            item.Discount)).ToList();

        return new OrderResponse(
            order.OrderId,
            order.CustomerId,
            order.OrderDate,
            order.TotalAmount,
            order.PaymentMethod,
            order.Status,
            items);
    }
}
