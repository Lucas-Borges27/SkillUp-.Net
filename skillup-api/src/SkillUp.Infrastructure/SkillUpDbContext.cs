using Microsoft.EntityFrameworkCore;
using SkillUp.Domain.Entities;

namespace SkillUp.Infrastructure;

public class SkillUpDbContext : DbContext
{
    public SkillUpDbContext(DbContextOptions<SkillUpDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Curso> Cursos => Set<Curso>();
    public DbSet<Progresso> Progressos => Set<Progresso>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SkillUpDbContext).Assembly);
    }
}
