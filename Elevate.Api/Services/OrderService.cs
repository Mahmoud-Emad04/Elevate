using Elevate.Api.Dtos;
using Elevate.Api.Models;
using Elevate.Api.Repositories;

namespace Elevate.Api.Services;

public class OrderService(
	ICustomerRepository customerRepository,
	IOrderRepository orderRepository,
	IProductRepository productRepository,
	IInvoiceRepository invoiceRepository,
	IEmailService emailService,
	IStripeService stripeService,
	ILogger<OrderService> logger
) : IOrderService
{
	public async Task<Order> CreateOrderAsync(OrderCreateRequest request, CancellationToken cancellationToken)
	{
		var customer = await customerRepository.GetAsync(request.CustomerId, cancellationToken);
		if (customer is null)
		{
			throw new InvalidOperationException("Customer not found.");
		}

		if (!Enum.TryParse<PaymentMethod>(request.PaymentMethod, true, out var paymentMethod))
		{
			throw new InvalidOperationException("Unsupported payment method.");
		}

		if (request.Items.Count == 0)
		{
			throw new InvalidOperationException("Order must contain at least one item.");
		}

		var orderItems = new List<OrderItem>();
		decimal subtotal = 0m;

		foreach (var item in request.Items)
		{
			var product = await productRepository.GetAsync(item.ProductId, cancellationToken);
			if (product is null)
			{
				throw new InvalidOperationException($"Product {item.ProductId} not found.");
			}

			if (product.Stock < item.Quantity)
			{
				throw new InvalidOperationException($"Insufficient stock for product {product.Name}.");
			}

			var lineTotal = product.Price * item.Quantity;
			subtotal += lineTotal;

			orderItems.Add(new OrderItem
			{
				ProductId = product.ProductId,
				Product = product,
				Quantity = item.Quantity,
				UnitPrice = product.Price,
				Discount = 0m
			});
		}

		var discountRate = subtotal switch
		{
			> 200m => 0.10m,
			> 100m => 0.05m,
			_ => 0m
		};

		foreach (var orderItem in orderItems)
		{
			var itemTotal = orderItem.UnitPrice * orderItem.Quantity;
			orderItem.Discount = Math.Round(itemTotal * discountRate, 2);
		}

		var totalDiscount = orderItems.Sum(item => item.Discount);
		var totalAmount = subtotal - totalDiscount;

		var order = new Order
		{
			CustomerId = customer.CustomerId,
			OrderDate = DateTime.UtcNow,
			PaymentMethod = paymentMethod,
			Status = OrderStatus.Pending,
			TotalAmount = totalAmount,
			OrderItems = orderItems
		};

		// Save order to get OrderId (required for both Stripe and non-Stripe payments)
		await orderRepository.AddAsync(order, cancellationToken);

		// Reduce product stock
		foreach (var orderItem in orderItems)
		{
			var product = await productRepository.GetAsync(orderItem.ProductId, cancellationToken);
			if (product is null)
			{
				continue;
			}

			product.Stock -= orderItem.Quantity;
			await productRepository.UpdateAsync(product, cancellationToken);
		}

		// Create invoice
		var invoice = new Invoice
		{
			OrderId = order.OrderId,
			InvoiceDate = DateTime.UtcNow,
			TotalAmount = totalAmount
		};

		await invoiceRepository.AddAsync(invoice, cancellationToken);
		order.Invoice = invoice;

		// Handle Stripe payment if payment method is Credit Card
		if (paymentMethod == PaymentMethod.CreditCard)
		{
			try
			{
				var checkoutUrl = await stripeService.CreateCheckoutSessionAsync(
					totalAmount,
					order.OrderId,  // ← Now has real OrderId from database!
					customer.Email,
					"usd");

				logger.LogInformation("Stripe checkout session created for order {OrderId}: {CheckoutUrl}", order.OrderId, checkoutUrl);

				// Return order with checkout URL for client to redirect to payment page
				// Order is already saved, Stripe will complete the payment
				return order;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Failed to create Stripe checkout session for order {OrderId}", order.OrderId);
				throw new InvalidOperationException("Failed to initiate payment. Please try again.", ex);
			}
		}

		// For PayPal or other methods, send confirmation email after order is created
		try
		{
			await emailService.SendOrderCreatedAsync(order, cancellationToken);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to send order confirmation email for order {OrderId}", order.OrderId);
			// Don't throw - email failure shouldn't block order completion
		}

		return order;
	}

	public Task<Order?> GetOrderAsync(int orderId, CancellationToken cancellationToken)
	{
		return orderRepository.GetAsync(orderId, cancellationToken);
	}

	public Task<List<Order>> GetOrdersAsync(CancellationToken cancellationToken)
	{
		return orderRepository.GetAllAsync(cancellationToken);
	}

	public Task<List<Order>> GetOrdersForCustomerAsync(int customerId, CancellationToken cancellationToken)
	{
		return orderRepository.GetByCustomerAsync(customerId, cancellationToken);
	}

	public async Task<Order?> UpdateStatusAsync(int orderId, OrderStatus status, CancellationToken cancellationToken)
	{
		var order = await orderRepository.GetAsync(orderId, cancellationToken);
		if (order is null)
		{
			return null;
		}

		order.Status = status;
		await orderRepository.UpdateAsync(order, cancellationToken);

		try
		{
			await emailService.SendOrderStatusChangedAsync(order, cancellationToken);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to send status change email for order {OrderId}", order.OrderId);
			// Don't throw - email failure shouldn't block status update
		}

		return order;
	}
}
