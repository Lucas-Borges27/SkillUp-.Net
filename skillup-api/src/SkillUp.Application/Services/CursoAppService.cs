using Microsoft.EntityFrameworkCore;
using SkillUp.Application.DTOs.Common;
using SkillUp.Application.DTOs.Cursos;
using SkillUp.Application.Extensions;
using SkillUp.Application.Interfaces;
using SkillUp.Domain.Entities;
using SkillUp.Domain.Enums;
using SkillUp.Domain.Exceptions;
using SkillUp.Domain.Repositories;

namespace SkillUp.Application.Services;

public class CursoAppService : ICursoAppService
{
    private readonly ICursoRepository _cursoRepository;

    public CursoAppService(ICursoRepository cursoRepository)
    {
        _cursoRepository = cursoRepository;
    }

    public async Task<PagedResponse<CursoResponse>> SearchAsync(CursoSearchQuery query, CancellationToken ct = default)
    {
        var page = query.NormalizePage();
        var size = query.NormalizeSize();
        var cursos = _cursoRepository.Query();

        if (!string.IsNullOrWhiteSpace(query.Filter))
        {
            var filter = query.Filter.ToLower();
            cursos = cursos.Where(c =>
                EF.Functions.Like(c.Nome.ToLower(), $"%{filter}%") ||
                (c.Descricao != null && EF.Functions.Like(c.Descricao.ToLower(), $"%{filter}%")));
        }

        if (!string.IsNullOrWhiteSpace(query.Categoria))
        {
            var categoria = query.Categoria.ToLower();
            cursos = cursos.Where(c => c.Categoria != null && EF.Functions.Like(c.Categoria.ToLower(), $"%{categoria}%"));
        }

        if (!string.IsNullOrWhiteSpace(query.Dificuldade) && Enum.TryParse<DificuldadeCurso>(query.Dificuldade, true, out var dificuldade))
        {
            cursos = cursos.Where(c => c.Dificuldade == dificuldade);
        }

        cursos = ApplySort(cursos, query.SortBy, query.SortDirection);

        var totalItems = await cursos.LongCountAsync(ct);
        var totalPages = (int)Math.Ceiling(totalItems / (double)size);
        var list = await cursos.Skip((page - 1) * size).Take(size).ToListAsync(ct);
        var dtos = list.Select(c => c.ToResponse()).ToList();

        var links = PaginationLinkBuilder.Build("/api/cursos/search", query, totalPages,
            new Dictionary<string, string?>
            {
                ["categoria"] = query.Categoria,
                ["dificuldade"] = query.Dificuldade
            });

        return new PagedResponse<CursoResponse>(dtos, new PaginationMetadata(page, size, totalPages, totalItems), links);
    }

    public async Task<ResourceResponse<CursoResponse>> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var curso = await _cursoRepository.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Curso não encontrado.");
        return new ResourceResponse<CursoResponse>(curso.ToResponse(), BuildCursoLinks(id));
    }

    public async Task<ResourceResponse<CursoResponse>> CreateAsync(CursoRequest request, CancellationToken ct = default)
    {
        var dificuldade = ParseDificuldade(request.Dificuldade);
        var curso = new Curso(request.Nome, request.Categoria, request.CargaHoraria, dificuldade, request.Descricao);
        await _cursoRepository.AddAsync(curso, ct);
        return new ResourceResponse<CursoResponse>(curso.ToResponse(), BuildCursoLinks(curso.Id));
    }

    public async Task<ResourceResponse<CursoResponse>> UpdateAsync(long id, CursoRequest request, CancellationToken ct = default)
    {
        var curso = await _cursoRepository.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Curso não encontrado.");
        var dificuldade = ParseDificuldade(request.Dificuldade);
        curso.AtualizarDados(request.Nome, request.Categoria, request.CargaHoraria, dificuldade, request.Descricao);
        await _cursoRepository.UpdateAsync(curso, ct);
        return new ResourceResponse<CursoResponse>(curso.ToResponse(), BuildCursoLinks(id));
    }

    public async Task DeleteAsync(long id, CancellationToken ct = default)
    {
        var curso = await _cursoRepository.GetByIdAsync(id, ct);
        if (curso is null)
        {
            throw new KeyNotFoundException("Curso não encontrado.");
        }

        await _cursoRepository.DeleteAsync(id, ct);
    }

    private static DificuldadeCurso ParseDificuldade(string dificuldade)
    {
        if (!Enum.TryParse<DificuldadeCurso>(dificuldade, true, out var parsed))
        {
            throw new DomainException("Dificuldade inválida.");
        }

        return parsed;
    }

    private static IQueryable<Curso> ApplySort(IQueryable<Curso> query, string? sortBy, string? direction)
    {
        var orderDesc = string.Equals(direction, "desc", StringComparison.OrdinalIgnoreCase);

        return sortBy?.ToLower() switch
        {
            "nome" => orderDesc ? query.OrderByDescending(c => c.Nome) : query.OrderBy(c => c.Nome),
            "cargahoraria" => orderDesc ? query.OrderByDescending(c => c.CargaHoraria) : query.OrderBy(c => c.CargaHoraria),
            "dificuldade" => orderDesc ? query.OrderByDescending(c => c.Dificuldade) : query.OrderBy(c => c.Dificuldade),
            _ => query.OrderBy(c => c.Id)
        };
    }

    private static IReadOnlyCollection<LinkResponse> BuildCursoLinks(long id)
    {
        return new[]
        {
            new LinkResponse("self", $"/api/cursos/{id}"),
            new LinkResponse("update", $"/api/cursos/{id}", "PUT"),
            new LinkResponse("delete", $"/api/cursos/{id}", "DELETE")
        };
    }
}
