using Elevate.Api.Dtos;
using Elevate.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Elevate.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(UserRegisterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await userService.RegisterAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(UserLoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await userService.LoginAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (InvalidOperationException)
        {
            return Unauthorized();
        }
    }
}

