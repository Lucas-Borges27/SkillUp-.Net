namespace SkillUp.Application.DTOs.Progresso;

public record ProgressoRequest(long UsuarioId, long CursoId, string? Status, decimal? Porcentagem, DateTime? DataInicio, DateTime? DataFim);
