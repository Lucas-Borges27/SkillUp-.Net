using SkillUp.Domain.Enums;
using SkillUp.Domain.Exceptions;

namespace SkillUp.Domain.Entities;

public class Progresso : Entity
{
    private Progresso()
    {
    }

    public Progresso(long usuarioId, long cursoId)
    {
        if (usuarioId <= 0 || cursoId <= 0)
        {
            throw new DomainException("Usuário e curso são obrigatórios para criar um progresso.");
        }

        UsuarioId = usuarioId;
        CursoId = cursoId;
        Status = StatusProgresso.NaoIniciado;
        Porcentagem = 0;
    }

    public long UsuarioId { get; private set; }
    public long CursoId { get; private set; }
    public StatusProgresso Status { get; private set; }
    public DateTime? DataInicio { get; private set; }
    public DateTime? DataFim { get; private set; }
    public decimal Porcentagem { get; private set; }

    public void Iniciar(DateTime? dataInicio = null)
    {
        if (Status != StatusProgresso.NaoIniciado)
        {
            throw new DomainException("Progresso já foi iniciado.");
        }

        DataInicio = dataInicio ?? DateTime.UtcNow;
        Status = StatusProgresso.EmAndamento;
        Porcentagem = 1;
    }

    public void AtualizarStatus(StatusProgresso novoStatus, DateTime? referencia = null)
    {
        switch (novoStatus)
        {
            case StatusProgresso.NaoIniciado:
                Resetar();
                break;
            case StatusProgresso.EmAndamento:
                if (DataInicio is null)
                {
                    DataInicio = referencia ?? DateTime.UtcNow;
                }
                Status = StatusProgresso.EmAndamento;
                if (Porcentagem <= 0)
                {
                    Porcentagem = 1;
                }
                DataFim = null;
                break;
            case StatusProgresso.Concluido:
                Concluir(referencia ?? DateTime.UtcNow);
                break;
            case StatusProgresso.EmRevisao:
                Status = StatusProgresso.EmRevisao;
                if (Porcentagem < 50)
                {
                    Porcentagem = 50;
                }
                break;
            default:
                throw new DomainException("Status de progresso inválido.");
        }
    }

    public void AtualizarPorcentagem(decimal porcentagem)
    {
        Porcentagem = ValidatePorcentagem(porcentagem);
        if (Porcentagem > 0 && DataInicio is null)
        {
            DataInicio = DateTime.UtcNow;
        }

        if (Porcentagem >= 100)
        {
            Concluir(DateTime.UtcNow);
        }
        else if (Status == StatusProgresso.NaoIniciado)
        {
            Status = StatusProgresso.EmAndamento;
        }
    }

    public void AjustarDatas(DateTime? dataInicio, DateTime? dataFim)
    {
        if (dataInicio.HasValue && dataFim.HasValue && dataFim < dataInicio)
        {
            throw new DomainException("Data de fim não pode ser menor que a data de início.");
        }

        DataInicio = dataInicio;
        DataFim = dataFim;
    }

    private void Concluir(DateTime dataFim)
    {
        if (DataInicio is null)
        {
            DataInicio = dataFim;
        }

        Porcentagem = 100;
        Status = StatusProgresso.Concluido;
        DataFim = dataFim;
    }

    private void Resetar()
    {
        Status = StatusProgresso.NaoIniciado;
        Porcentagem = 0;
        DataInicio = null;
        DataFim = null;
    }

    private static decimal ValidatePorcentagem(decimal porcentagem)
    {
        if (porcentagem < 0 || porcentagem > 100)
        {
            throw new DomainException("Porcentagem deve estar entre 0 e 100.");
        }

        return Math.Round(porcentagem, 2, MidpointRounding.AwayFromZero);
    }
}
