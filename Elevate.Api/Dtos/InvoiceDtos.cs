namespace Elevate.Api.Dtos;

public record InvoiceResponse(int InvoiceId, int OrderId, DateTime InvoiceDate, decimal TotalAmount);
