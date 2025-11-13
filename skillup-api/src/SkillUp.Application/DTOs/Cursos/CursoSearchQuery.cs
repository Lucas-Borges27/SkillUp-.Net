using SkillUp.Application.DTOs.Common;

namespace SkillUp.Application.DTOs.Cursos;

public record CursoSearchQuery : PaginationQuery
{
    public string? Categoria { get; init; }
    public string? Dificuldade { get; init; }
}
