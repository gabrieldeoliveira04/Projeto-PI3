using GestaoFerias.Application.DTOs;
using GestaoFerias.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestaoFerias.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
        => Ok(new { token = await _authService.Register(request) });

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
        => Ok(new { token = await _authService.Login(request) });
}
