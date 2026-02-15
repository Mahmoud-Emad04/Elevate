using Elevate.Api.Dtos;
using Elevate.Api.Models;
using Elevate.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elevate.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController(IProductService productService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ProductResponse>>> GetProducts(CancellationToken cancellationToken)
    {
        var response = await productService.GetAllProductsAsync(cancellationToken);
        return Ok(response);
    }

    [HttpGet("{productId}")]
    public async Task<ActionResult<ProductResponse>> GetProduct([FromRoute] int productId, CancellationToken cancellationToken)
    {
        var product = await productService.GetProductAsync(productId, cancellationToken);
        if (product is null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpPost]
    public async Task<ActionResult<ProductResponse>> AddProduct(ProductCreateRequest request, CancellationToken cancellationToken)
    {
        var product = await productService.AddProductAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetProduct), new { productId = product.ProductId }, product);
    }

    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpPut("{productId}")]
    public async Task<ActionResult<ProductResponse>> UpdateProduct([FromRoute] int productId, [FromBody] ProductUpdateRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await productService.UpdateProductAsync(productId, request, cancellationToken);
            return Ok(product);
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }
}
