namespace GestaoFerias.Application.DTOs;

public class UpdateUserRequest
{
    public string Nome { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string? Senha { get; set; } // opcional
}
