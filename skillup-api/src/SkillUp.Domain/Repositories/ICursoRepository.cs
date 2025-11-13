using SkillUp.Domain.Entities;

namespace SkillUp.Domain.Repositories;

public interface ICursoRepository
{
    IQueryable<Curso> Query();
    Task<Curso?> GetByIdAsync(long id, CancellationToken ct = default);
    Task AddAsync(Curso curso, CancellationToken ct = default);
    Task UpdateAsync(Curso curso, CancellationToken ct = default);
    Task DeleteAsync(long id, CancellationToken ct = default);
}
