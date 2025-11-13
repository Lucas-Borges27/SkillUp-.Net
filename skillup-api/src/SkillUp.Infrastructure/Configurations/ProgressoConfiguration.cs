using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillUp.Domain.Entities;
using SkillUp.Domain.Enums;

namespace SkillUp.Infrastructure.Configurations;

public class ProgressoConfiguration : IEntityTypeConfiguration<Progresso>
{
    public void Configure(EntityTypeBuilder<Progresso> builder)
    {
        builder.ToTable("SKILLUP_PROGRESSO");

        builder.HasKey(p => p.Id)
            .HasName("PK_SKILLUP_PROGRESSO");

        builder.Property(p => p.Id)
            .HasColumnName("ID_PROGRESSO")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.UsuarioId)
            .HasColumnName("ID_USUARIO")
            .IsRequired();

        builder.Property(p => p.CursoId)
            .HasColumnName("ID_CURSO")
            .IsRequired();

        builder.Property(p => p.Status)
            .HasColumnName("STATUS")
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(StatusProgresso.NaoIniciado)
            .IsRequired();

        builder.Property(p => p.DataInicio)
            .HasColumnName("DATA_INICIO")
            .HasColumnType("DATE");

        builder.Property(p => p.DataFim)
            .HasColumnName("DATA_FIM")
            .HasColumnType("DATE");

        builder.Property(p => p.Porcentagem)
            .HasColumnName("PORCENTAGEM")
            .HasColumnType("NUMBER(5,2)")
            .HasDefaultValue(0)
            .IsRequired();

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(p => p.UsuarioId)
            .HasConstraintName("FK_PROGRESSO_USUARIO");

        builder.HasOne<Curso>()
            .WithMany()
            .HasForeignKey(p => p.CursoId)
            .HasConstraintName("FK_PROGRESSO_CURSO");

        builder.HasIndex(p => new { p.UsuarioId, p.CursoId })
            .IsUnique()
            .HasDatabaseName("UX_PROGRESSO_USUARIO_CURSO");
    }
}
