using System.ComponentModel.DataAnnotations;

namespace Elevate.Api.Dtos;

public record CustomerCreateRequest(
     string Name,
     string Email);

public record CustomerResponse(int CustomerId, string Name, string Email);
