using Elevate.Api.Models;

namespace Elevate.Api.Services;

public interface IEmailService
{
    Task SendOrderCreatedAsync(Order order, CancellationToken cancellationToken);
    Task SendOrderStatusChangedAsync(Order order, CancellationToken cancellationToken);
}
