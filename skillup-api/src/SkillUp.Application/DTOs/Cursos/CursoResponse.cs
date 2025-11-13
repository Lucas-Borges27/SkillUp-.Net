namespace SkillUp.Application.DTOs.Cursos;

public record CursoResponse(long Id, string Nome, string? Categoria, int CargaHoraria, string Dificuldade, string? Descricao);
