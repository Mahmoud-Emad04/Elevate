using Stripe;
using Stripe.Checkout;

namespace Elevate.Api.Services;

public class StripeService : IStripeService
{
	private readonly ILogger<StripeService> _logger;

	public StripeService(IConfiguration configuration, ILogger<StripeService> logger)
	{
		_logger = logger;
		StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];
	}

	public async Task<string> CreateCheckoutSessionAsync(decimal amount, int orderId, string customerEmail, string currency = "usd")
	{
		try
		{
			var options = new SessionCreateOptions
			{
				PaymentMethodTypes = new List<string> { "card" },
				LineItems = new List<SessionLineItemOptions>
				{
					new SessionLineItemOptions
					{
						PriceData = new SessionLineItemPriceDataOptions
						{
							UnitAmount = (long)(amount * 100), // Convert to cents
							Currency = currency,
							ProductData = new SessionLineItemPriceDataProductDataOptions
							{
								Name = $"Order #{orderId}",
								Description = $"Payment for order {orderId}",
							},
						},
						Quantity = 1,
					},
				},
				Mode = "payment",
				SuccessUrl = $"https://localhost:7228/api/payments/success?session_id={{CHECKOUT_SESSION_ID}}&order_id={orderId}",
				CancelUrl = $"https://localhost:7228/api/payments/cancel?order_id={orderId}",
				CustomerEmail = customerEmail,
				Metadata = new Dictionary<string, string>
				{
					{ "order_id", orderId.ToString() },
					{ "customer_email", customerEmail }
				}
			};

			var service = new SessionService();
			var session = await service.CreateAsync(options);

			_logger.LogInformation("Stripe checkout session created for order {OrderId}: {SessionId}", orderId, session.Id);
			return session.Url;
		}
		catch (StripeException ex)
		{
			_logger.LogError(ex, "Stripe error creating checkout session for order {OrderId}", orderId);
			throw;
		}
	}

	public async Task<(bool Success, string Message)> GetPaymentIntentStatusAsync(string paymentIntentId)
	{
		try
		{
			var service = new PaymentIntentService();
			var paymentIntent = await service.GetAsync(paymentIntentId);

			if (paymentIntent.Status == "succeeded")
			{
				_logger.LogInformation("Payment intent {PaymentIntentId} succeeded", paymentIntentId);
				return (true, "Payment successful");
			}

			_logger.LogWarning("Payment intent {PaymentIntentId} status: {Status}", paymentIntentId, paymentIntent.Status);
			return (false, $"Payment status: {paymentIntent.Status}");
		}
		catch (StripeException ex)
		{
			_logger.LogError(ex, "Stripe error retrieving payment intent {PaymentIntentId}", paymentIntentId);
			return (false, "Failed to retrieve payment status");
		}
	}

	public async Task<(bool Success, string PaymentIntentId, string Message)> CreatePaymentIntentAsync(decimal amount, string paymentMethodId, string customerEmail)
	{
		try
		{
			var options = new PaymentIntentCreateOptions
			{
				Amount = (long)(amount * 100), // Convert to cents
				Currency = "usd",
				PaymentMethod = paymentMethodId,
				ConfirmationMethod = "automatic",
				Confirm = true,
				ReturnUrl = "https://localhost:7228/api/payments/success",
				ReceiptEmail = customerEmail,
				Description = $"Order payment for {customerEmail}",
			};

			var service = new PaymentIntentService();
			var paymentIntent = await service.CreateAsync(options);

			if (paymentIntent.Status == "succeeded")
			{
				_logger.LogInformation("Payment intent {PaymentIntentId} created and succeeded", paymentIntent.Id);
				return (true, paymentIntent.Id, "Payment successful");
			}

			_logger.LogWarning("Payment intent {PaymentIntentId} created with status: {Status}", paymentIntent.Id, paymentIntent.Status);
			return (false, paymentIntent.Id, $"Payment status: {paymentIntent.Status}");
		}
		catch (StripeException ex)
		{
			_logger.LogError(ex, "Stripe error creating payment intent for {CustomerEmail}", customerEmail);
			return (false, string.Empty, $"Payment error: {ex.Message}");
		}
	}
}
