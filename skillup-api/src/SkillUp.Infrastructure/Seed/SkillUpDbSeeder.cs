using Microsoft.EntityFrameworkCore;
using SkillUp.Domain.Entities;
using SkillUp.Domain.Enums;

namespace SkillUp.Infrastructure.Seed;

public static class SkillUpDbSeeder
{
    public static async Task SeedAsync(SkillUpDbContext context, CancellationToken ct = default)
    {
        if (await context.Usuarios.AnyAsync(ct))
        {
            return;
        }

        var usuarios = new List<Usuario>
        {
            new("Ana Silva", "ana@skillup.dev", "SenhaSegura1", "Dados"),
            new("Bruno Costa", "bruno@skillup.dev", "SenhaSegura1", "Backend"),
            new("Camila Ramos", "camila@skillup.dev", "SenhaSegura1", "Cloud")
        };

        foreach (var usuario in usuarios)
        {
            usuario.DefinirDataCadastro(DateTime.UtcNow.AddDays(-usuarios.IndexOf(usuario) * 3));
        }

        var cursos = new List<Curso>
        {
            new("Introdução a Oracle SQL", "Banco de Dados", 20, DificuldadeCurso.Basico, "Comece no SQL com exercícios práticos."),
            new("APIs com ASP.NET Core", "Backend", 30, DificuldadeCurso.Intermediario, "Crie APIs maduras em .NET."),
            new("Fundamentos de Cloud", "DevOps", 18, DificuldadeCurso.Basico, "Entenda os principais providers de nuvem."),
            new("Machine Learning aplicado", "Dados", 40, DificuldadeCurso.Avancado, "Construa pipelines completos de ML."),
        };

        await context.Usuarios.AddRangeAsync(usuarios, ct);
        await context.Cursos.AddRangeAsync(cursos, ct);
        await context.SaveChangesAsync(ct);

        var progressos = new List<Progresso>
        {
            new(usuarios[0].Id, cursos[0].Id),
            new(usuarios[1].Id, cursos[1].Id),
            new(usuarios[2].Id, cursos[2].Id)
        };

        progressos[0].AtualizarPorcentagem(65);
        progressos[1].AtualizarStatus(StatusProgresso.EmAndamento);
        progressos[1].AtualizarPorcentagem(25);
        progressos[2].AtualizarStatus(StatusProgresso.Concluido, DateTime.UtcNow.AddDays(-2));

        await context.Progressos.AddRangeAsync(progressos, ct);
        await context.SaveChangesAsync(ct);
    }
}
