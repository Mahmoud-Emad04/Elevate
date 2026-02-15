using System.ComponentModel.DataAnnotations;
using Elevate.Api.Models;

namespace Elevate.Api.Dtos;

public record OrderItemRequest(
    int ProductId,
     int Quantity);

public record OrderCreateRequest(
    int CustomerId,
    string PaymentMethod,
    List<OrderItemRequest> Items);

public record OrderItemResponse(int ProductId, string ProductName, int Quantity, decimal UnitPrice, decimal Discount);

public record OrderResponse(int OrderId, int CustomerId, DateTime OrderDate, decimal TotalAmount, PaymentMethod PaymentMethod, OrderStatus Status, List<OrderItemResponse> Items);

public record UpdateOrderStatusRequest(OrderStatus Status);
