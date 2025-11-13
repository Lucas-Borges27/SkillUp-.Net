namespace SkillUp.Application.DTOs.Progresso;

public record ProgressoResponse(long Id, long UsuarioId, long CursoId, string Status, decimal Porcentagem, DateTime? DataInicio, DateTime? DataFim);
