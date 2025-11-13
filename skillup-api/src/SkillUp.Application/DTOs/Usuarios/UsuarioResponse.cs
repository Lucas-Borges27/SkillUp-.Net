namespace SkillUp.Application.DTOs.Usuarios;

public record UsuarioResponse(long Id, string Nome, string Email, string? AreaInteresse, DateTime DataCadastro);
