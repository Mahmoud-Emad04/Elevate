using Stripe.Checkout;

namespace Elevate.Api.Services;

public interface IStripeService
{
    /// <summary>
    /// Creates a Stripe checkout session for payment
    /// </summary>
    /// <param name="amount">Amount in dollars (will be converted to cents)</param>
    /// <param name="orderId">Order ID for reference</param>
    /// <param name="customerEmail">Customer email for receipt</param>
    /// <param name="currency">Currency code (default: usd)</param>
    /// <returns>Checkout URL</returns>
    Task<string> CreateCheckoutSessionAsync(decimal amount, int orderId, string customerEmail, string currency = "usd");

    /// <summary>
    /// Retrieves payment intent details
    /// </summary>
    /// <param name="paymentIntentId">Stripe Payment Intent ID</param>
    /// <returns>Payment Intent details</returns>
    Task<(bool Success, string Message)> GetPaymentIntentStatusAsync(string paymentIntentId);

    /// <summary>
    /// Confirms a payment intent (for direct charge)
    /// </summary>
    /// <param name="amount">Amount in dollars (will be converted to cents)</param>
    /// <param name="paymentMethodId">Stripe Payment Method ID</param>
    /// <param name="customerEmail">Customer email</param>
    /// <returns>Payment Intent ID if successful</returns>
    Task<(bool Success, string PaymentIntentId, string Message)> CreatePaymentIntentAsync(decimal amount, string paymentMethodId, string customerEmail);
}
