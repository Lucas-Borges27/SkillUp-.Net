using SkillUp.Application.DTOs.Common;

namespace SkillUp.Application.DTOs.Progresso;

public record ProgressoSearchQuery : PaginationQuery
{
    public long? UsuarioId { get; init; }
    public long? CursoId { get; init; }
    public string? Status { get; init; }
}
