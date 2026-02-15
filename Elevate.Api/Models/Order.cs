namespace Elevate.Api.Models;

public class Order
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderItem> OrderItems { get; set; } = [];
    public PaymentMethod PaymentMethod { get; set; }
    public OrderStatus Status { get; set; }
    public Invoice? Invoice { get; set; }
    public string? StripeSessionId { get; set; }
    public string? StripePaymentIntentId { get; set; }
}
