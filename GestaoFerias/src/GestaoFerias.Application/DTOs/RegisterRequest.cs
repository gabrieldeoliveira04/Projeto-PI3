namespace GestaoFerias.Application.DTOs;

public class RegisterRequest
{
    public string Nome { get; set; } = null!;
    public string Senha { get; set; } = null!;
    public string Role { get; set; } = null!;
}
