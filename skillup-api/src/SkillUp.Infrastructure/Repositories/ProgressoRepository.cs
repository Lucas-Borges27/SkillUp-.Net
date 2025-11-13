using Microsoft.EntityFrameworkCore;
using SkillUp.Domain.Entities;
using SkillUp.Domain.Repositories;

namespace SkillUp.Infrastructure.Repositories;

public class ProgressoRepository : IProgressoRepository
{
    private readonly SkillUpDbContext _context;

    public ProgressoRepository(SkillUpDbContext context)
    {
        _context = context;
    }

    public IQueryable<Progresso> Query() => _context.Progressos.AsNoTracking();

    public async Task<Progresso?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await _context.Progressos.FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<Progresso?> GetByUsuarioCursoAsync(long usuarioId, long cursoId, CancellationToken ct = default)
    {
        return await _context.Progressos.FirstOrDefaultAsync(p => p.UsuarioId == usuarioId && p.CursoId == cursoId, ct);
    }

    public async Task AddAsync(Progresso progresso, CancellationToken ct = default)
    {
        await _context.Progressos.AddAsync(progresso, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Progresso progresso, CancellationToken ct = default)
    {
        _context.Progressos.Update(progresso);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(long id, CancellationToken ct = default)
    {
        var entity = await _context.Progressos.FindAsync(new object?[] { id }, ct);
        if (entity is not null)
        {
            _context.Progressos.Remove(entity);
            await _context.SaveChangesAsync(ct);
        }
    }
}
