using Elevate.Api.Dtos;
using Elevate.Api.Models;

namespace Elevate.Api.Services;

public interface IInvoiceService
{
    Task<InvoiceResponse?> GetInvoiceAsync(int invoiceId, CancellationToken cancellationToken);
    Task<List<InvoiceResponse>> GetAllInvoicesAsync(CancellationToken cancellationToken);
}
