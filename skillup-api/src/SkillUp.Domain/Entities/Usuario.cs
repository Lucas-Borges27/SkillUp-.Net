using System.Net.Mail;
using SkillUp.Domain.Exceptions;

namespace SkillUp.Domain.Entities;

public class Usuario : Entity
{
    private Usuario()
    {
    }

    public Usuario(string nome, string email, string senha, string? areaInteresse)
    {
        AtualizarPerfil(nome, email, areaInteresse);
        AlterarSenha(senha);
        DataCadastro = DateTime.UtcNow;
    }

    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Senha { get; private set; } = string.Empty;
    public string? AreaInteresse { get; private set; }
    public DateTime DataCadastro { get; private set; }

    public void AtualizarPerfil(string nome, string email, string? areaInteresse)
    {
        Nome = ValidateNome(nome);
        Email = ValidateEmail(email);
        AreaInteresse = string.IsNullOrWhiteSpace(areaInteresse) ? null : areaInteresse.Trim();
    }

    public void AlterarSenha(string senha)
    {
        if (string.IsNullOrWhiteSpace(senha))
        {
            throw new DomainException("Senha é obrigatória.");
        }

        var trimmed = senha.Trim();
        if (trimmed.Length < 8)
        {
            throw new DomainException("Senha deve possuir no mínimo 8 caracteres.");
        }

        Senha = trimmed;
    }

    public void DefinirDataCadastro(DateTime dataCadastro)
    {
        if (dataCadastro > DateTime.UtcNow.AddMinutes(1))
        {
            throw new DomainException("Data de cadastro não pode estar no futuro.");
        }

        DataCadastro = dataCadastro;
    }

    private static string ValidateNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new DomainException("Nome é obrigatório.");
        }

        var trimmed = nome.Trim();
        if (trimmed.Length < 3)
        {
            throw new DomainException("Nome deve possuir no mínimo 3 caracteres.");
        }

        return trimmed;
    }

    private static string ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new DomainException("Email é obrigatório.");
        }

        try
        {
            var addr = new MailAddress(email.Trim());
            return addr.Address.ToLowerInvariant();
        }
        catch
        {
            throw new DomainException("Email inválido.");
        }
    }
}
