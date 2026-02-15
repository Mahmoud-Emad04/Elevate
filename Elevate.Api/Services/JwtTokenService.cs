using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Elevate.Api.Models;
using Microsoft.IdentityModel.Tokens;

namespace Elevate.Api.Services;

public class JwtTokenService(IConfiguration configuration) : IJwtTokenService
{
    public string GenerateToken(User user)
    {
        var key = configuration["Jwt:Key"] ?? "dev-secret-key-change-please-use-longer-key";
        var issuer = configuration["Jwt:Issuer"] ?? "Elevate.Api";
        var audience = configuration["Jwt:Audience"] ?? "Elevate.Api";

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
