using SkillUp.Domain.Entities;

namespace SkillUp.Domain.Repositories;

public interface IProgressoRepository
{
    IQueryable<Progresso> Query();
    Task<Progresso?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<Progresso?> GetByUsuarioCursoAsync(long usuarioId, long cursoId, CancellationToken ct = default);
    Task AddAsync(Progresso progresso, CancellationToken ct = default);
    Task UpdateAsync(Progresso progresso, CancellationToken ct = default);
    Task DeleteAsync(long id, CancellationToken ct = default);
}
