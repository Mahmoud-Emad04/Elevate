using Elevate.Api.Data;
using Elevate.Api.Dtos;
using Elevate.Api.Models;
using Elevate.Api.Repositories;
using Elevate.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Elevate.Api.Tests;

public class OrderServiceTests
{
    [Fact]
    public async Task CreateOrderAsync_AppliesTieredDiscount()
    {
        var options = new DbContextOptionsBuilder<OrderManagementDbContext>()
            .UseInMemoryDatabase($"OrderDb_{Guid.NewGuid()}")
            .Options;

        await using var dbContext = new OrderManagementDbContext(options);
        var customerRepository = new CustomerRepository(dbContext);
        var orderRepository = new OrderRepository(dbContext);
        var productRepository = new ProductRepository(dbContext);
        var invoiceRepository = new InvoiceRepository(dbContext);
        var emailService = new TestEmailService();
        var stripeService = new Mock<IStripeService>();
        var logger = new Mock<ILogger<OrderService>>();

        var orderService = new OrderService(customerRepository, orderRepository, productRepository, invoiceRepository, emailService, stripeService.Object, logger.Object);

        var customer = await customerRepository.AddAsync(new Customer { Name = "Test", Email = "test@example.com" }, CancellationToken.None);
        await productRepository.AddAsync(new Product { Name = "Item", Price = 120m, Stock = 10 }, CancellationToken.None);

        var request = new OrderCreateRequest(customer.CustomerId, PaymentMethod.PayPal.ToString(),
        [
            new OrderItemRequest(1, 2)
        ]);

        var order = await orderService.CreateOrderAsync(request, CancellationToken.None);

        Assert.Equal(216m, order.TotalAmount);
    }

    [Fact]
    public async Task CreateOrderAsync_ThrowsWhenStockIsInsufficient()
    {
        var options = new DbContextOptionsBuilder<OrderManagementDbContext>()
            .UseInMemoryDatabase($"OrderDb_{Guid.NewGuid()}")
            .Options;

        await using var dbContext = new OrderManagementDbContext(options);
        var customerRepository = new CustomerRepository(dbContext);
        var orderRepository = new OrderRepository(dbContext);
        var productRepository = new ProductRepository(dbContext);
        var invoiceRepository = new InvoiceRepository(dbContext);
        var emailService = new TestEmailService();
        var stripeService = new Mock<IStripeService>();
        var logger = new Mock<ILogger<OrderService>>();

        var orderService = new OrderService(customerRepository, orderRepository, productRepository, invoiceRepository, emailService, stripeService.Object, logger.Object);

        var customer = await customerRepository.AddAsync(new Customer { Name = "Test", Email = "test@example.com" }, CancellationToken.None);
        await productRepository.AddAsync(new Product { Name = "Item", Price = 30m, Stock = 1 }, CancellationToken.None);

        var request = new OrderCreateRequest(customer.CustomerId, PaymentMethod.PayPal.ToString(),
        [
            new OrderItemRequest(1, 2)
        ]);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            orderService.CreateOrderAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateStatusAsync_UpdatesOrderStatus()
    {
        var options = new DbContextOptionsBuilder<OrderManagementDbContext>()
            .UseInMemoryDatabase($"OrderDb_{Guid.NewGuid()}")
            .Options;

        await using var dbContext = new OrderManagementDbContext(options);
        var customerRepository = new CustomerRepository(dbContext);
        var orderRepository = new OrderRepository(dbContext);
        var productRepository = new ProductRepository(dbContext);
        var invoiceRepository = new InvoiceRepository(dbContext);
        var emailService = new TestEmailService();
        var stripeService = new Mock<IStripeService>();
        var logger = new Mock<ILogger<OrderService>>();

        var orderService = new OrderService(customerRepository, orderRepository, productRepository, invoiceRepository, emailService, stripeService.Object, logger.Object);

        var customer = await customerRepository.AddAsync(new Customer { Name = "Test", Email = "test@example.com" }, CancellationToken.None);
        await productRepository.AddAsync(new Product { Name = "Item", Price = 10m, Stock = 5 }, CancellationToken.None);

        var request = new OrderCreateRequest(customer.CustomerId, PaymentMethod.PayPal.ToString(),
        [
            new OrderItemRequest(1, 1)
        ]);

        var order = await orderService.CreateOrderAsync(request, CancellationToken.None);
        var updated = await orderService.UpdateStatusAsync(order.OrderId, OrderStatus.Completed, CancellationToken.None);

        Assert.NotNull(updated);
        Assert.Equal(OrderStatus.Completed, updated!.Status);
    }

    private sealed class TestEmailService : IEmailService
    {
        public Task SendOrderCreatedAsync(Order order, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task SendOrderStatusChangedAsync(Order order, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
