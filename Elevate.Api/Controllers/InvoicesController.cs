using Elevate.Api.Dtos;
using Elevate.Api.Models;
using Elevate.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elevate.Api.Controllers;

[ApiController]
[Route("api/invoices")]
[Authorize(Roles = nameof(UserRole.Admin))]
public class InvoicesController(IInvoiceService invoiceService) : ControllerBase
{
    [HttpGet("{invoiceId}")]
    public async Task<ActionResult<InvoiceResponse>> GetInvoice([FromRoute] int invoiceId, CancellationToken cancellationToken)
    {
        var invoice = await invoiceService.GetInvoiceAsync(invoiceId, cancellationToken);
        if (invoice is null)
        {
            return NotFound();
        }

        return Ok(invoice);
    }

    [HttpGet]
    public async Task<ActionResult<List<InvoiceResponse>>> GetInvoices(CancellationToken cancellationToken)
    {
        var response = await invoiceService.GetAllInvoicesAsync(cancellationToken);
        return Ok(response);
    }
}

