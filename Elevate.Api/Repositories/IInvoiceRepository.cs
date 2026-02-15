using Elevate.Api.Models;

namespace Elevate.Api.Repositories;

public interface IInvoiceRepository
{
    Task<Invoice?> GetAsync(int invoiceId, CancellationToken cancellationToken);
    Task<List<Invoice>> GetAllAsync(CancellationToken cancellationToken);
    Task<Invoice> AddAsync(Invoice invoice, CancellationToken cancellationToken);
}
