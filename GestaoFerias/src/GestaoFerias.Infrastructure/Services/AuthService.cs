using GestaoFerias.Application.DTOs;
using GestaoFerias.Application.Interfaces;
using GestaoFerias.Domain.Entities;
using GestaoFerias.Domain.Enums;
using GestaoFerias.Infrastructure.Auth;
using GestaoFerias.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace GestaoFerias.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly TokenService _tokenService;

    public AuthService(AppDbContext context, TokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    // ==========================
    // GERA MATRÍCULA AUTOMÁTICA
    // ==========================
    private async Task<string> GerarMatriculaAsync()
    {
        var ultimaMatricula = await _context.Usuarios
            .OrderByDescending(u => u.Matricula)
            .Select(u => u.Matricula)
            .FirstOrDefaultAsync();

        int nova = string.IsNullOrEmpty(ultimaMatricula)
            ? 1
            : int.Parse(ultimaMatricula) + 1;

        if (nova > 9999)
            throw new Exception("Limite de matrículas atingido.");

        return nova.ToString("D4"); // 0001, 0002...
    }

    // ==========================
    // REGISTER
    // ==========================
    public async Task<(string Matricula, string Token)> Register(RegisterRequest request)
    {
        var matricula = await GerarMatriculaAsync();

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Matricula = matricula,
            Nome = request.Nome,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Senha),
            Role = Enum.Parse<UserRole>(request.Role, true)
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        try
{
    _context.Usuarios.Add(usuario);
    await _context.SaveChangesAsync();
}
catch (DbUpdateException)
{
    throw new Exception("Erro ao gerar matrícula. Tente novamente.");
}


        var token = _tokenService.GenerateToken(usuario);

        return (usuario.Matricula, token);
    }

    // ==========================
    // LOGIN
    // ==========================
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
