using SkillUp.Application.DTOs.Progresso;
using SkillUp.Domain.Entities;

namespace SkillUp.Application.Extensions;

public static class ProgressoExtensions
{
    public static ProgressoResponse ToResponse(this Progresso progresso) =>
        new(progresso.Id, progresso.UsuarioId, progresso.CursoId, progresso.Status.ToString(),
            progresso.Porcentagem, progresso.DataInicio, progresso.DataFim);
}
