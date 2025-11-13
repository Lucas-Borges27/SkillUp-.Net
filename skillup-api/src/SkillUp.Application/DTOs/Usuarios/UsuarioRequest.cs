namespace SkillUp.Application.DTOs.Usuarios;

public record UsuarioRequest(string Nome, string Email, string Senha, string? AreaInteresse);
