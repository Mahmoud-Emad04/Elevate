using Elevate.Api.Models;

namespace Elevate.Api.Repositories;

public interface IProductRepository
{
    Task<Product?> GetAsync(int productId, CancellationToken cancellationToken);
    Task<List<Product>> GetAllAsync(CancellationToken cancellationToken);
    Task<Product> AddAsync(Product product, CancellationToken cancellationToken);
    Task UpdateAsync(Product product, CancellationToken cancellationToken);
}
