using Elevate.Api.Models;
using Elevate.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elevate.Api.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController(
    IOrderService orderService,
    IStripeService stripeService,
    ILogger<PaymentsController> logger) : ControllerBase
{
    /// <summary>
    /// Handles successful payment redirect from Stripe
    /// </summary>
    [AllowAnonymous]
    [HttpGet("success")]
    public async Task<IActionResult> PaymentSuccess([FromQuery(Name = "session_id")] string sessionId, [FromQuery(Name = "order_id")] int orderId, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Payment success callback for order {OrderId} with session {SessionId}", orderId, sessionId);

            // Retrieve order and verify payment
            var order = await orderService.GetOrderAsync(orderId, cancellationToken);
            if (order is null)
            {
                return BadRequest("Order not found.");
            }

            // Store session ID for reference
            order.StripeSessionId = sessionId;

            // Update order status to Processing
            var updatedOrder = await orderService.UpdateStatusAsync(orderId, OrderStatus.Processing, cancellationToken);
            if (updatedOrder is null)
            {
                return BadRequest("Failed to update order.");
            }

            return Ok(new
            {
                success = true,
                message = "Payment successful! Your order is being processed.",
                orderId = orderId,
                status = updatedOrder.Status.ToString()
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing payment success for order {OrderId}", orderId);
            return BadRequest($"Error processing payment: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles cancelled payment from Stripe
    /// </summary>
    [AllowAnonymous]
    [HttpGet("cancel")]
    public async Task<IActionResult> PaymentCancel([FromQuery(Name = "order_id")] int orderId, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogWarning("Payment cancelled for order {OrderId}", orderId);

            var order = await orderService.GetOrderAsync(orderId, cancellationToken);
            if (order is not null)
            {
                // Update order status to Cancelled
                await orderService.UpdateStatusAsync(orderId, OrderStatus.Cancelled, cancellationToken);
            }

            return Ok(new
            {
                success = false,
                message = "Payment was cancelled. You can try again later.",
                orderId = orderId
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing payment cancellation for order {OrderId}", orderId);
            return BadRequest($"Error processing cancellation: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets Stripe checkout URL for an order
    /// </summary>
    [Authorize]
    [HttpGet("{orderId}/checkout-url")]
    public async Task<IActionResult> GetCheckoutUrl([FromRoute] int orderId, CancellationToken cancellationToken)
    {
        try
        {
            var order = await orderService.GetOrderAsync(orderId, cancellationToken);
            if (order is null)
            {
                return NotFound("Order not found.");
            }

            if (order.PaymentMethod != PaymentMethod.CreditCard)
            {
                return BadRequest("This endpoint is only for credit card payments.");
            }

            // Create a new checkout session
            var checkoutUrl = await stripeService.CreateCheckoutSessionAsync(
                order.TotalAmount,
                orderId,
                order.Customer?.Email ?? string.Empty,
                "usd");

            return Ok(new
            {
                orderId = orderId,
                checkoutUrl = checkoutUrl,
                amount = order.TotalAmount,
                currency = "USD"
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting checkout URL for order {OrderId}", orderId);
            return BadRequest($"Error creating checkout session: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets payment status for an order
    /// </summary>
    [Authorize]
    [HttpGet("{orderId}/status")]
    public async Task<IActionResult> GetPaymentStatus([FromRoute] int orderId, CancellationToken cancellationToken)
    {
        try
        {
            var order = await orderService.GetOrderAsync(orderId, cancellationToken);
            if (order is null)
            {
                return NotFound("Order not found.");
            }

            if (string.IsNullOrEmpty(order.StripePaymentIntentId))
            {
                return Ok(new
                {
                    orderId = orderId,
                    status = order.Status.ToString(),
                    message = "No payment intent associated with this order."
                });
            }

            var (success, message) = await stripeService.GetPaymentIntentStatusAsync(order.StripePaymentIntentId);

            return Ok(new
            {
                orderId = orderId,
                status = order.Status.ToString(),
                paymentSuccess = success,
                message = message
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting payment status for order {OrderId}", orderId);
            return BadRequest($"Error retrieving payment status: {ex.Message}");
        }
    }
}
