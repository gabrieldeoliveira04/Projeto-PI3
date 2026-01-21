using GestaoFerias.Application.DTOs;
using GestaoFerias.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using GestaoFerias.Infrastructure.Auth;


namespace GestaoFerias.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
public async Task<IActionResult> Register(RegisterRequest request)
{
    var result = await _authService.Register(request);

    return Ok(new
    {
        matricula = result.Matricula,
        token = result.Token
    });
}


    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
        => Ok(new { token = await _authService.Login(request) });
}
