using Microsoft.EntityFrameworkCore;
using SkillUp.Domain.Entities;
using SkillUp.Domain.Repositories;

namespace SkillUp.Infrastructure.Repositories;

public class CursoRepository : ICursoRepository
{
    private readonly SkillUpDbContext _context;

    public CursoRepository(SkillUpDbContext context)
    {
        _context = context;
    }

    public IQueryable<Curso> Query() => _context.Cursos.AsNoTracking();

    public async Task<Curso?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await _context.Cursos.FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task AddAsync(Curso curso, CancellationToken ct = default)
    {
        await _context.Cursos.AddAsync(curso, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Curso curso, CancellationToken ct = default)
    {
        _context.Cursos.Update(curso);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(long id, CancellationToken ct = default)
    {
        var entity = await _context.Cursos.FindAsync(new object?[] { id }, ct);
        if (entity is not null)
        {
            _context.Cursos.Remove(entity);
            await _context.SaveChangesAsync(ct);
        }
    }
}
