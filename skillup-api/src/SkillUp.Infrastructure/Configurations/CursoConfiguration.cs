using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillUp.Domain.Entities;

namespace SkillUp.Infrastructure.Configurations;

public class CursoConfiguration : IEntityTypeConfiguration<Curso>
{
    public void Configure(EntityTypeBuilder<Curso> builder)
    {
        builder.ToTable("SKILLUP_CURSO");

        builder.HasKey(c => c.Id)
            .HasName("PK_SKILLUP_CURSO");

        builder.Property(c => c.Id)
            .HasColumnName("ID_CURSO")
            .ValueGeneratedOnAdd();

        builder.Property(c => c.Nome)
            .HasColumnName("NOME")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Categoria)
            .HasColumnName("CATEGORIA")
            .HasMaxLength(50);

        builder.Property(c => c.CargaHoraria)
            .HasColumnName("CARGA_HORARIA")
            .HasColumnType("NUMBER")
            .IsRequired();

        builder.Property(c => c.Dificuldade)
            .HasColumnName("DIFICULDADE")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(c => c.Descricao)
            .HasColumnName("DESCRICAO")
            .HasMaxLength(255);
    }
}
