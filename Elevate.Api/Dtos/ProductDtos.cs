using System.ComponentModel.DataAnnotations;

namespace Elevate.Api.Dtos;

public record ProductCreateRequest(
    string Name,
    decimal Price,
    int Stock);

public record ProductUpdateRequest(
    string Name,
    decimal Price,
    int Stock);

public record ProductResponse(int ProductId, string Name, decimal Price, int Stock);
