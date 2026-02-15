using Elevate.Api.Data;
using Elevate.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Elevate.Api.Repositories;

public class ProductRepository(OrderManagementDbContext dbContext) : IProductRepository
{
    public Task<Product?> GetAsync(int productId, CancellationToken cancellationToken)
    {
        return dbContext.Products.FirstOrDefaultAsync(product => product.ProductId == productId, cancellationToken);
    }

    public Task<List<Product>> GetAllAsync(CancellationToken cancellationToken)
    {
        return dbContext.Products.ToListAsync(cancellationToken);
    }

    public async Task<Product> AddAsync(Product product, CancellationToken cancellationToken)
    {
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task UpdateAsync(Product product, CancellationToken cancellationToken)
    {
        dbContext.Products.Update(product);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
