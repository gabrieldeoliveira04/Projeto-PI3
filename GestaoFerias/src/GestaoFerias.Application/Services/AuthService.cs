using GestaoFerias.Application.DTOs;
using GestaoFerias.Domain.Entities;
using GestaoFerias.Domain.Enums;
using GestaoFerias.Infrastructure.Auth;
using GestaoFerias.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace GestaoFerias.Application.Services;

public class AuthService
{
    private readonly AppDbContext _context;
    private readonly TokenService _tokenService;

    public AuthService(AppDbContext context, TokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<string> Register(RegisterRequest request)
    {
        if (await _context.Usuarios.AnyAsync(u => u.Matricula == request.Matricula))
            throw new Exception("Matrícula já cadastrada.");

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Matricula = request.Matricula,
            Nome = request.Nome,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Senha),
            Role = Enum.Parse<UserRole>(request.Role, true)
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return _tokenService.GenerateToken(usuario);
    }

    public async Task<string> Login(LoginRequest request)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Matricula == request.Matricula);

        if (usuario == null ||
            !BCrypt.Net.BCrypt.Verify(request.Senha, usuario.PasswordHash))
            throw new Exception("Credenciais inválidas.");

        return _tokenService.GenerateToken(usuario);
    }
}
