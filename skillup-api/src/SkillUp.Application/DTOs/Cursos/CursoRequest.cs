namespace SkillUp.Application.DTOs.Cursos;

public record CursoRequest(string Nome, string? Categoria, int CargaHoraria, string Dificuldade, string? Descricao);
