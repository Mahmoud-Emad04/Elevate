using Elevate.Api.Models;

namespace Elevate.Api.Services;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}
