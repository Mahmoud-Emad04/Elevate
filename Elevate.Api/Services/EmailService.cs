using System.Net;
using System.Net.Mail;
using Elevate.Api.Configuration;
using Elevate.Api.Models;
using Microsoft.Extensions.Options;

namespace Elevate.Api.Services;

public class EmailService(IOptions<MailSettings> mailSettings, ILogger<EmailService> logger) : IEmailService
{
    private readonly MailSettings _mailSettings = mailSettings.Value;

    public async Task SendOrderCreatedAsync(Order order, CancellationToken cancellationToken)
    {
        try
        {
            var customer = order.Customer;
            if (customer is null)
            {
                logger.LogWarning("Cannot send email: Customer not found for order {OrderId}", order.OrderId);
                return;
            }

            var subject = $"Order Confirmation - Order #{order.OrderId}";
            var htmlContent = await LoadTemplate("OrderCreated.html");
            
            htmlContent = htmlContent
                .Replace("{{CustomerName}}", customer.Name)
                .Replace("{{OrderId}}", order.OrderId.ToString())
                .Replace("{{OrderDate}}", order.OrderDate.ToString("MMMM dd, yyyy"))
                .Replace("{{PaymentMethod}}", order.PaymentMethod.ToString())
                .Replace("{{Status}}", order.Status.ToString())
                .Replace("{{TotalAmount}}", order.TotalAmount.ToString("C"))
                .Replace("{{TrackingUrl}}", "https://your-domain.com/orders/{{OrderId}}")
                .Replace("{{OrderItems}}", GenerateOrderItemsHtml(order));

            await SendEmailAsync(customer.Email, subject, htmlContent, cancellationToken);
            logger.LogInformation("Order created email sent successfully to {Email} for order {OrderId}", customer.Email, order.OrderId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error sending order created email for order {OrderId}", order.OrderId);
        }
    }

    public async Task SendOrderStatusChangedAsync(Order order, CancellationToken cancellationToken)
    {
        try
        {
            var customer = order.Customer;
            if (customer is null)
            {
                logger.LogWarning("Cannot send email: Customer not found for order {OrderId}", order.OrderId);
                return;
            }

            var subject = $"Order Status Update - Order #{order.OrderId} is {order.Status}";
            var htmlContent = await LoadTemplate("OrderStatusChanged.html");
            
            var statusClass = order.Status switch
            {
                OrderStatus.Pending => "pending",
                OrderStatus.Processing => "processing",
                OrderStatus.Completed => "completed",
                OrderStatus.Cancelled => "cancelled",
                _ => "pending"
            };

            htmlContent = htmlContent
                .Replace("{{CustomerName}}", customer.Name)
                .Replace("{{OrderId}}", order.OrderId.ToString())
                .Replace("{{Status}}", order.Status.ToString())
                .Replace("{{StatusClass}}", statusClass)
                .Replace("{{UpdatedDate}}", DateTime.UtcNow.ToString("MMMM dd, yyyy h:mm tt"))
                .Replace("{{TrackingUrl}}", "https://your-domain.com/orders/{{OrderId}}")
                .Replace("{{StatusTimeline}}", GenerateStatusTimeline(order.Status));

            await SendEmailAsync(customer.Email, subject, htmlContent, cancellationToken);
            logger.LogInformation("Order status changed email sent successfully to {Email} for order {OrderId}", customer.Email, order.OrderId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error sending order status changed email for order {OrderId}", order.OrderId);
        }
    }

    private async Task SendEmailAsync(string toEmail, string subject, string htmlContent, CancellationToken cancellationToken)
    {
        using var client = new SmtpClient(_mailSettings.Host, _mailSettings.Port)
        {
            Credentials = new NetworkCredential(_mailSettings.Mail, _mailSettings.Password),
            EnableSsl = true,
            Timeout = 10000
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_mailSettings.Mail, _mailSettings.DisplayName),
            Subject = subject,
            Body = htmlContent,
            IsBodyHtml = true
        };

        mailMessage.To.Add(toEmail);

        await client.SendMailAsync(mailMessage, cancellationToken);
    }

    private async Task<string> LoadTemplate(string templateName)
    {
        var templatePath = Path.Combine(AppContext.BaseDirectory, "Templates", templateName);
        
        if (!File.Exists(templatePath))
        {
            logger.LogWarning("Email template not found at {TemplatePath}", templatePath);
            return string.Empty;
        }

        return await File.ReadAllTextAsync(templatePath);
    }

    private string GenerateOrderItemsHtml(Order order)
    {
        var html = string.Empty;

        foreach (var item in order.OrderItems)
        {
            var itemTotal = (item.Quantity * item.UnitPrice) - item.Discount;
            html += $@"
                <tr>
                    <td>{item.Product?.Name}</td>
                    <td>{item.Quantity}</td>
                    <td>{item.UnitPrice:C}</td>
                    <td>{item.Discount:C}</td>
                    <td>{itemTotal:C}</td>
                </tr>";
        }

        return html;
    }

    private string GenerateStatusTimeline(OrderStatus status)
    {
        var timeline = string.Empty;
        var statuses = new[] { OrderStatus.Pending, OrderStatus.Processing, OrderStatus.Completed };

        foreach (var s in statuses)
        {
            var isCompleted = (int)s <= (int)status;
            var markerClass = isCompleted ? "completed" : "";
            
            timeline += $@"
                <div class=""timeline-item"">
                    <div class=""timeline-marker {markerClass}""></div>
                    <div class=""timeline-content"">
                        <strong>{s}</strong>
                        <p>{GetStatusDescription(s)}</p>
                    </div>
                </div>";
        }

        return timeline;
    }

    private string GetStatusDescription(OrderStatus status)
    {
        return status switch
        {
            OrderStatus.Pending => "Your order has been received and is awaiting processing.",
            OrderStatus.Processing => "Your order is being prepared and will be shipped soon.",
            OrderStatus.Completed => "Your order has been completed and delivered.",
            OrderStatus.Cancelled => "Your order has been cancelled.",
            _ => "Status update pending."
        };
    }
}

