using SkillUp.Application.DTOs.Cursos;
using SkillUp.Domain.Entities;

namespace SkillUp.Application.Extensions;

public static class CursoExtensions
{
    public static CursoResponse ToResponse(this Curso curso) =>
        new(curso.Id, curso.Nome, curso.Categoria, curso.CargaHoraria, curso.Dificuldade.ToString(), curso.Descricao);
}
