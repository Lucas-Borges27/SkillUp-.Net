using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillUp.Domain.Entities;

namespace SkillUp.Infrastructure.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("SKILLUP_USUARIO");

        builder.HasKey(u => u.Id)
            .HasName("PK_SKILLUP_USUARIO");

        builder.Property(u => u.Id)
            .HasColumnName("ID_USUARIO")
            .ValueGeneratedOnAdd();

        builder.Property(u => u.Nome)
            .HasColumnName("NOME")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.Email)
            .HasColumnName("EMAIL")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.Senha)
            .HasColumnName("SENHA")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.AreaInteresse)
            .HasColumnName("AREA_INTERESSE")
            .HasMaxLength(50);

        builder.Property(u => u.DataCadastro)
            .HasColumnName("DATA_CADASTRO")
            .HasColumnType("DATE")
            .HasDefaultValueSql("SYSDATE");

        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("UX_SKILLUP_USUARIO_EMAIL");
    }
}
