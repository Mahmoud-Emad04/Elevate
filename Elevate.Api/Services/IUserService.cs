using Elevate.Api.Dtos;

namespace Elevate.Api.Services;

public interface IUserService
{
    Task<AuthResponse> RegisterAsync(UserRegisterRequest request, CancellationToken cancellationToken);
    Task<AuthResponse> LoginAsync(UserLoginRequest request, CancellationToken cancellationToken);
}
