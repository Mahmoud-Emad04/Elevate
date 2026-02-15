using System.ComponentModel.DataAnnotations;
using Elevate.Api.Models;

namespace Elevate.Api.Dtos;

public record UserRegisterRequest(
     string Username,
     string Password,
      UserRole Role);

public record UserLoginRequest(
    string Username,
    string Password);

public record AuthResponse(string Token, string Role);
