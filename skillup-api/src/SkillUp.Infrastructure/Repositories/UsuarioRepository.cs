using Microsoft.EntityFrameworkCore;
using SkillUp.Domain.Entities;
using SkillUp.Domain.Repositories;

namespace SkillUp.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly SkillUpDbContext _context;

    public UsuarioRepository(SkillUpDbContext context)
    {
        _context = context;
    }

    public IQueryable<Usuario> Query() => _context.Usuarios.AsNoTracking();

    public async Task<Usuario?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == normalized, ct);
    }

    public async Task AddAsync(Usuario usuario, CancellationToken ct = default)
    {
        await _context.Usuarios.AddAsync(usuario, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Usuario usuario, CancellationToken ct = default)
    {
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(long id, CancellationToken ct = default)
    {
        var entity = await _context.Usuarios.FindAsync(new object?[] { id }, ct);
        if (entity is not null)
        {
            _context.Usuarios.Remove(entity);
            await _context.SaveChangesAsync(ct);
        }
    }
}
