using Elevate.Api.Configuration;
using Elevate.Api.Data;
using Elevate.Api.Extensions;
using Elevate.Api.Models;
using Elevate.Api.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

builder.Services.AddCustomControllers();
builder.Services.AddOpenApi();
builder.Services.AddFluentValidationServices();

builder.Services.AddDbContext<OrderManagementDbContext>(options =>
    options.UseInMemoryDatabase("OrderManagementDb"));

builder.Services.AddRepositories();
builder.Services.AddBusinessServices();
builder.Services.AddUtilityServices();
builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<OrderManagementDbContext>();
    SeedDatabase(dbContext, scope.ServiceProvider);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Elevate Api")
            .WithTheme(ScalarTheme.Purple)
            .WithClassicLayout()
            .WithOpenApiRoutePattern("/openapi/v1.json");
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();


void SeedDatabase(OrderManagementDbContext dbContext, IServiceProvider serviceProvider)
{
    if (!dbContext.Products.Any())
    {
        dbContext.Products.AddRange(
            new Product { Name = "test 1", Price = 25m, Stock = 100 },
            new Product { Name = "test 2", Price = 75m, Stock = 50 },
            new Product { Name = "test 3", Price = 150m, Stock = 25 });
    }

    if (!dbContext.Customers.Any())
    {
        dbContext.Customers.AddRange(
            new Customer { Name = "Alice", Email = "alice@gmail.com" },
            new Customer { Name = "Bob", Email = "bob@gmail.com" });
    }

    if (!dbContext.Users.Any())
    {
        var hasher = serviceProvider.GetRequiredService<IPasswordHasher>();
        dbContext.Users.AddRange(
            new User { Username = "admin", PasswordHash = hasher.Hash("Admin123!"), Role = UserRole.Admin },
            new User { Username = "customer", PasswordHash = hasher.Hash("Customer123!"), Role = UserRole.Customer });
    }

    dbContext.SaveChanges();
}
