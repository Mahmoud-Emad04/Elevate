using Elevate.Api.Dtos;
using Elevate.Api.Models;
using Elevate.Api.Repositories;

namespace Elevate.Api.Services;

public class ProductService(IProductRepository productRepository) : IProductService
{
    public async Task<List<ProductResponse>> GetAllProductsAsync(CancellationToken cancellationToken)
    {
        var products = await productRepository.GetAllAsync(cancellationToken);
        return products
            .Select(p => new ProductResponse(p.ProductId, p.Name, p.Price, p.Stock))
            .ToList();
    }

    public async Task<ProductResponse?> GetProductAsync(int productId, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetAsync(productId, cancellationToken);
        if (product is null)
        {
            return null;
        }

        return new ProductResponse(product.ProductId, product.Name, product.Price, product.Stock);
    }

    public async Task<ProductResponse> AddProductAsync(ProductCreateRequest request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name,
            Price = request.Price,
            Stock = request.Stock
        };

        await productRepository.AddAsync(product, cancellationToken);
        return new ProductResponse(product.ProductId, product.Name, product.Price, product.Stock);
    }

    public async Task<ProductResponse> UpdateProductAsync(int productId, ProductUpdateRequest request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetAsync(productId, cancellationToken);
        if (product is null)
        {
            throw new InvalidOperationException($"Product with ID {productId} not found.");
        }

        product.Name = request.Name;
        product.Price = request.Price;
        product.Stock = request.Stock;

        await productRepository.UpdateAsync(product, cancellationToken);
        return new ProductResponse(product.ProductId, product.Name, product.Price, product.Stock);
    }
}
