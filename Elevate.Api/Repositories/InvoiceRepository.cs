using Elevate.Api.Data;
using Elevate.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Elevate.Api.Repositories;

public class InvoiceRepository(OrderManagementDbContext dbContext) : IInvoiceRepository
{
    public Task<Invoice?> GetAsync(int invoiceId, CancellationToken cancellationToken)
    {
        return dbContext.Invoices.FirstOrDefaultAsync(invoice => invoice.InvoiceId == invoiceId, cancellationToken);
    }

    public Task<List<Invoice>> GetAllAsync(CancellationToken cancellationToken)
    {
        return dbContext.Invoices.ToListAsync(cancellationToken);
    }

    public async Task<Invoice> AddAsync(Invoice invoice, CancellationToken cancellationToken)
    {
        dbContext.Invoices.Add(invoice);
        await dbContext.SaveChangesAsync(cancellationToken);
        return invoice;
    }
}
