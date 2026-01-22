using GestaoFerias.Application.DTOs;
using GestaoFerias.Application.Interfaces;
using GestaoFerias.Domain.Entities;
using GestaoFerias.Domain.Enums;
using GestaoFerias.Infrastructure.Auth;
using GestaoFerias.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace GestaoFerias.Infrastructure.Services;

public class UsuarioService : IUsuarioService
{
    private readonly AppDbContext _context;

    public UsuarioService(AppDbContext context)
    {
        _context = context;
    }

    private async Task<string> GerarMatriculaAsync()
    {
        var ultima = await _context.Usuarios
            .OrderByDescending(u => u.Matricula)
            .Select(u => u.Matricula)
            .FirstOrDefaultAsync();

        int nova = string.IsNullOrEmpty(ultima) ? 1 : int.Parse(ultima) + 1;

        if (nova > 9999)
            throw new Exception("Limite de matrículas atingido.");

        return nova.ToString("D4");
    }

    public async Task<IEnumerable<UserResponse>> GetAll()
        => await _context.Usuarios
            .Select(u => new UserResponse
            {
                Id = u.Id,
                Matricula = u.Matricula,
                Nome = u.Nome,
                Role = u.Role.ToString()
            })
            .ToListAsync();

    public async Task<UserResponse> GetById(Guid id)
    {
        var usuario = await _context.Usuarios.FindAsync(id)
            ?? throw new Exception("Usuário não encontrado.");

        return new UserResponse
        {
            Id = usuario.Id,
            Matricula = usuario.Matricula,
            Nome = usuario.Nome,
            Role = usuario.Role.ToString()
        };
    }

    public async Task<UserResponse> GetByMatricula(string matricula)
{
    var usuario = await _context.Usuarios
        .AsNoTracking()
        .FirstOrDefaultAsync(u => u.Matricula == matricula)
        ?? throw new Exception("Usuário não encontrado.");

    return new UserResponse
    {
        Id = usuario.Id,
        Matricula = usuario.Matricula,
        Nome = usuario.Nome,
        Role = usuario.Role.ToString()
    };
}

public async Task<IEnumerable<UserResponse>> GetByNome(string nome)
{
    return await _context.Usuarios
        .AsNoTracking()
        .Where(u => EF.Functions.ILike(u.Nome, $"%{nome}%"))
        .Select(u => new UserResponse
        {
            Id = u.Id,
            Matricula = u.Matricula,
            Nome = u.Nome,
            Role = u.Role.ToString()
        })
        .ToListAsync();
}



    public async Task Update(Guid id, UpdateUserRequest request)
{
    var usuario = await _context.Usuarios.FindAsync(id)
        ?? throw new Exception("Usuário não encontrado.");

    usuario.Nome = request.Nome;
    usuario.Role = Enum.Parse<UserRole>(request.Role, true);

    if (!string.IsNullOrWhiteSpace(request.Senha))
    {
        usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Senha);
    }

    await _context.SaveChangesAsync();
}

public async Task UpdateByMatricula(string matricula, UpdateUserRequest request)
{
    var usuario = await _context.Usuarios
        .FirstOrDefaultAsync(u => u.Matricula == matricula)
        ?? throw new Exception("Usuário não encontrado.");

    usuario.Nome = request.Nome;
    usuario.Role = Enum.Parse<UserRole>(request.Role, true);

    if (!string.IsNullOrWhiteSpace(request.Senha))
    {
        usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Senha);
    }

    await _context.SaveChangesAsync();
}

    public async Task Delete(Guid id)
    {
        var usuario = await _context.Usuarios.FindAsync(id)
            ?? throw new Exception("Usuário não encontrado.");

        _context.Usuarios.Remove(usuario);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteByMatricula(string matricula)
{
    var usuario = await _context.Usuarios
        .FirstOrDefaultAsync(u => u.Matricula == matricula)
        ?? throw new Exception("Usuário não encontrado.");

    _context.Usuarios.Remove(usuario);
    await _context.SaveChangesAsync();
}

}
