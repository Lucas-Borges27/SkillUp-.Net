using SkillUp.Domain.Entities;

namespace SkillUp.Domain.Repositories;

public interface IUsuarioRepository
{
    IQueryable<Usuario> Query();
    Task<Usuario?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task AddAsync(Usuario usuario, CancellationToken ct = default);
    Task UpdateAsync(Usuario usuario, CancellationToken ct = default);
    Task DeleteAsync(long id, CancellationToken ct = default);
}
