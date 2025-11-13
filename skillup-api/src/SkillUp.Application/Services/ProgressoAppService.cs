using Microsoft.EntityFrameworkCore;
using SkillUp.Application.DTOs.Common;
using SkillUp.Application.DTOs.Progresso;
using SkillUp.Application.Extensions;
using SkillUp.Application.Interfaces;
using SkillUp.Domain.Entities;
using SkillUp.Domain.Enums;
using SkillUp.Domain.Exceptions;
using SkillUp.Domain.Repositories;

namespace SkillUp.Application.Services;

public class ProgressoAppService : IProgressoAppService
{
    private readonly IProgressoRepository _progressoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICursoRepository _cursoRepository;

    public ProgressoAppService(IProgressoRepository progressoRepository, IUsuarioRepository usuarioRepository, ICursoRepository cursoRepository)
    {
        _progressoRepository = progressoRepository;
        _usuarioRepository = usuarioRepository;
        _cursoRepository = cursoRepository;
    }

    public async Task<PagedResponse<ProgressoResponse>> SearchAsync(ProgressoSearchQuery query, CancellationToken ct = default)
    {
        var page = query.NormalizePage();
        var size = query.NormalizeSize();
        var progressoQuery = _progressoRepository.Query();

        if (query.UsuarioId.HasValue)
        {
            progressoQuery = progressoQuery.Where(p => p.UsuarioId == query.UsuarioId.Value);
        }

        if (query.CursoId.HasValue)
        {
            progressoQuery = progressoQuery.Where(p => p.CursoId == query.CursoId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Status) && Enum.TryParse<StatusProgresso>(query.Status, true, out var status))
        {
            progressoQuery = progressoQuery.Where(p => p.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(query.Filter))
        {
            var filter = query.Filter.ToLower();
            progressoQuery = progressoQuery.Where(p =>
                EF.Functions.Like(p.Status.ToString().ToLower(), $"%{filter}%"));
        }

        progressoQuery = ApplySort(progressoQuery, query.SortBy, query.SortDirection);

        var totalItems = await progressoQuery.LongCountAsync(ct);
        var totalPages = (int)Math.Ceiling(totalItems / (double)size);
        var dtos = (await progressoQuery.Skip((page - 1) * size).Take(size).ToListAsync(ct)).Select(p => p.ToResponse()).ToList();

        var links = PaginationLinkBuilder.Build("/api/progresso/search", query, totalPages,
            new Dictionary<string, string?>
            {
                ["usuarioId"] = query.UsuarioId?.ToString(),
                ["cursoId"] = query.CursoId?.ToString(),
                ["status"] = query.Status
            });

        return new PagedResponse<ProgressoResponse>(dtos, new PaginationMetadata(page, size, totalPages, totalItems), links);
    }

    public async Task<ResourceResponse<ProgressoResponse>> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var progresso = await _progressoRepository.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Progresso não encontrado.");
        return new ResourceResponse<ProgressoResponse>(progresso.ToResponse(), BuildProgressoLinks(id));
    }

    public async Task<ResourceResponse<ProgressoResponse>> CreateAsync(ProgressoRequest request, CancellationToken ct = default)
    {
        await EnsureUsuarioAndCursoExistAsync(request.UsuarioId, request.CursoId, ct);

        var existente = await _progressoRepository.GetByUsuarioCursoAsync(request.UsuarioId, request.CursoId, ct);
        if (existente is not null)
        {
            throw new DomainException("Já existe progresso registrado para este usuário e curso.");
        }

        var progresso = new Progresso(request.UsuarioId, request.CursoId);
        ApplyStateFromRequest(progresso, request);
        await _progressoRepository.AddAsync(progresso, ct);
        return new ResourceResponse<ProgressoResponse>(progresso.ToResponse(), BuildProgressoLinks(progresso.Id));
    }

    public async Task<ResourceResponse<ProgressoResponse>> UpdateAsync(long id, ProgressoRequest request, CancellationToken ct = default)
    {
        await EnsureUsuarioAndCursoExistAsync(request.UsuarioId, request.CursoId, ct);

        var progresso = await _progressoRepository.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Progresso não encontrado.");
        if (progresso.UsuarioId != request.UsuarioId || progresso.CursoId != request.CursoId)
        {
            throw new DomainException("Não é permitido alterar o relacionamento usuário/curso em um progresso já existente.");
        }

        ApplyStateFromRequest(progresso, request);
        await _progressoRepository.UpdateAsync(progresso, ct);
        return new ResourceResponse<ProgressoResponse>(progresso.ToResponse(), BuildProgressoLinks(id));
    }

    public async Task DeleteAsync(long id, CancellationToken ct = default)
    {
        var progresso = await _progressoRepository.GetByIdAsync(id, ct);
        if (progresso is null)
        {
            throw new KeyNotFoundException("Progresso não encontrado.");
        }

        await _progressoRepository.DeleteAsync(id, ct);
    }

    private async Task EnsureUsuarioAndCursoExistAsync(long usuarioId, long cursoId, CancellationToken ct)
    {
        _ = await _usuarioRepository.GetByIdAsync(usuarioId, ct) ?? throw new DomainException("Usuário informado não existe.");
        _ = await _cursoRepository.GetByIdAsync(cursoId, ct) ?? throw new DomainException("Curso informado não existe.");
    }

    private static void ApplyStateFromRequest(Progresso progresso, ProgressoRequest request)
    {
        if (request.DataInicio.HasValue || request.DataFim.HasValue)
        {
            progresso.AjustarDatas(request.DataInicio, request.DataFim);
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (!Enum.TryParse<StatusProgresso>(request.Status, true, out var status))
            {
                throw new DomainException("Status inválido.");
            }

            progresso.AtualizarStatus(status);
        }

        if (request.Porcentagem.HasValue)
        {
            progresso.AtualizarPorcentagem(request.Porcentagem.Value);
        }
    }

    private static IQueryable<Progresso> ApplySort(IQueryable<Progresso> query, string? sortBy, string? direction)
    {
        var orderDesc = string.Equals(direction, "desc", StringComparison.OrdinalIgnoreCase);

        return sortBy?.ToLower() switch
        {
            "porcentagem" => orderDesc ? query.OrderByDescending(p => p.Porcentagem) : query.OrderBy(p => p.Porcentagem),
            "datainicio" => orderDesc ? query.OrderByDescending(p => p.DataInicio) : query.OrderBy(p => p.DataInicio),
            "datafim" => orderDesc ? query.OrderByDescending(p => p.DataFim) : query.OrderBy(p => p.DataFim),
            _ => query.OrderBy(p => p.Id)
        };
    }

    private static IReadOnlyCollection<LinkResponse> BuildProgressoLinks(long id)
    {
        return new[]
        {
            new LinkResponse("self", $"/api/progresso/{id}"),
            new LinkResponse("update", $"/api/progresso/{id}", "PUT"),
            new LinkResponse("delete", $"/api/progresso/{id}", "DELETE")
        };
    }
}
