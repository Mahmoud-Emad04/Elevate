namespace Elevate.Api.Models;

public class Invoice
{
    public int InvoiceId { get; set; }
    public int OrderId { get; set; }
    public Order? Order { get; set; }
    public DateTime InvoiceDate { get; set; }
    public decimal TotalAmount { get; set; }
}
