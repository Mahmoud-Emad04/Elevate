using Elevate.Api.Dtos;
using Elevate.Api.Repositories;

namespace Elevate.Api.Services;

public class InvoiceService(IInvoiceRepository invoiceRepository) : IInvoiceService
{
    public async Task<InvoiceResponse?> GetInvoiceAsync(int invoiceId, CancellationToken cancellationToken)
    {
        var invoice = await invoiceRepository.GetAsync(invoiceId, cancellationToken);
        if (invoice is null)
        {
            return null;
        }

        return new InvoiceResponse(invoice.InvoiceId, invoice.OrderId, invoice.InvoiceDate, invoice.TotalAmount);
    }

    public async Task<List<InvoiceResponse>> GetAllInvoicesAsync(CancellationToken cancellationToken)
    {
        var invoices = await invoiceRepository.GetAllAsync(cancellationToken);
        return invoices
            .Select(i => new InvoiceResponse(i.InvoiceId, i.OrderId, i.InvoiceDate, i.TotalAmount))
            .ToList();
    }
}
