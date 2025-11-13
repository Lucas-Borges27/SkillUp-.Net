using SkillUp.Domain.Enums;
using SkillUp.Domain.Exceptions;

namespace SkillUp.Domain.Entities;

public class Curso : Entity
{
    private Curso()
    {
    }

    public Curso(string nome, string? categoria, int cargaHoraria, DificuldadeCurso dificuldade, string? descricao)
    {
        AtualizarDados(nome, categoria, cargaHoraria, dificuldade, descricao);
    }

    public string Nome { get; private set; } = string.Empty;
    public string? Categoria { get; private set; }
    public int CargaHoraria { get; private set; }
    public DificuldadeCurso Dificuldade { get; private set; }
    public string? Descricao { get; private set; }

    public void AtualizarDados(string nome, string? categoria, int cargaHoraria, DificuldadeCurso dificuldade, string? descricao)
    {
        Nome = ValidateNome(nome);
        Categoria = string.IsNullOrWhiteSpace(categoria) ? null : categoria.Trim();
        CargaHoraria = ValidateCargaHoraria(cargaHoraria);
        Dificuldade = dificuldade;
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
    }

    private static string ValidateNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new DomainException("Nome do curso é obrigatório.");
        }

        var trimmed = nome.Trim();
        if (trimmed.Length < 3)
        {
            throw new DomainException("Nome do curso deve possuir no mínimo 3 caracteres.");
        }

        return trimmed;
    }

    private static int ValidateCargaHoraria(int cargaHoraria)
    {
        if (cargaHoraria <= 0)
        {
            throw new DomainException("Carga horária deve ser maior que zero.");
        }

        if (cargaHoraria > 1000)
        {
            throw new DomainException("Carga horária inválida.");
        }

        return cargaHoraria;
    }
}
