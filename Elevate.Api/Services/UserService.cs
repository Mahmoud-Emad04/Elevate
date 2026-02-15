using Elevate.Api.Dtos;
using Elevate.Api.Models;
using Elevate.Api.Repositories;

namespace Elevate.Api.Services;

public class UserService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService) : IUserService
{
    public async Task<AuthResponse> RegisterAsync(UserRegisterRequest request, CancellationToken cancellationToken)
    {
        var existingUser = await userRepository.GetByUsernameAsync(request.Username, cancellationToken);
        if (existingUser is not null)
        {
            throw new InvalidOperationException("Username already exists.");
        }

        var user = new User
        {
            Username = request.Username,
            PasswordHash = passwordHasher.Hash(request.Password),
            Role = request.Role
        };

        await userRepository.AddAsync(user, cancellationToken);
        var token = jwtTokenService.GenerateToken(user);
        return new AuthResponse(token, user.Role.ToString());
    }

    public async Task<AuthResponse> LoginAsync(UserLoginRequest request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByUsernameAsync(request.Username, cancellationToken);
        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new InvalidOperationException("Invalid username or password.");
        }

        var token = jwtTokenService.GenerateToken(user);
        return new AuthResponse(token, user.Role.ToString());
    }
}
