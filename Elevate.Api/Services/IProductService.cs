using Elevate.Api.Dtos;
using Elevate.Api.Models;

namespace Elevate.Api.Services;

public interface IProductService
{
    Task<List<ProductResponse>> GetAllProductsAsync(CancellationToken cancellationToken);
    Task<ProductResponse?> GetProductAsync(int productId, CancellationToken cancellationToken);
    Task<ProductResponse> AddProductAsync(ProductCreateRequest request, CancellationToken cancellationToken);
    Task<ProductResponse> UpdateProductAsync(int productId, ProductUpdateRequest request, CancellationToken cancellationToken);
}
