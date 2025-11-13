using SkillUp.Application.DTOs.Common;

namespace SkillUp.Application.DTOs.Usuarios;

public record UsuarioSearchQuery : PaginationQuery
{
    public string? AreaInteresse { get; init; }
}
