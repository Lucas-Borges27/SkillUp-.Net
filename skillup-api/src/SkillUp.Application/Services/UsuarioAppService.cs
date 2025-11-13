using Microsoft.EntityFrameworkCore;
using SkillUp.Application.DTOs.Common;
using SkillUp.Application.DTOs.Usuarios;
using SkillUp.Application.Extensions;
using SkillUp.Application.Interfaces;
using SkillUp.Domain.Entities;
using SkillUp.Domain.Exceptions;
using SkillUp.Domain.Repositories;

namespace SkillUp.Application.Services;

public class UsuarioAppService : IUsuarioAppService
{
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuarioAppService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<PagedResponse<UsuarioResponse>> SearchAsync(UsuarioSearchQuery query, CancellationToken ct = default)
    {
        var page = query.NormalizePage();
        var size = query.NormalizeSize();

        var usuarios = _usuarioRepository.Query();

        if (!string.IsNullOrWhiteSpace(query.Filter))
        {
            usuarios = usuarios.Where(u =>
                EF.Functions.Like(u.Nome.ToLower(), $"%{query.Filter.ToLower()}%") ||
                EF.Functions.Like(u.Email.ToLower(), $"%{query.Filter.ToLower()}%"));
        }

        if (!string.IsNullOrWhiteSpace(query.AreaInteresse))
        {
            var area = query.AreaInteresse.ToLower();
            usuarios = usuarios.Where(u => u.AreaInteresse != null && EF.Functions.Like(u.AreaInteresse.ToLower(), $"%{area}%"));
        }

        usuarios = ApplySort(usuarios, query.SortBy, query.SortDirection);

        var totalItems = await usuarios.LongCountAsync(ct);
        var totalPages = (int)Math.Ceiling(totalItems / (double)size);
        var items = await usuarios.Skip((page - 1) * size).Take(size).ToListAsync(ct);
        var dtoList = items.Select(u => u.ToResponse()).ToList();

        var links = PaginationLinkBuilder.Build("/api/usuarios/search", query, totalPages, new Dictionary<string, string?>
        {
            ["areaInteresse"] = query.AreaInteresse
        });

        return new PagedResponse<UsuarioResponse>(dtoList, new PaginationMetadata(page, size, totalPages, totalItems), links);
    }

    public async Task<ResourceResponse<UsuarioResponse>> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id, ct);
        if (usuario is null)
        {
            throw new KeyNotFoundException("Usuário não encontrado.");
        }

        return new ResourceResponse<UsuarioResponse>(usuario.ToResponse(), BuildUsuarioLinks(id));
    }

    public async Task<ResourceResponse<UsuarioResponse>> CreateAsync(UsuarioRequest request, CancellationToken ct = default)
    {
        ValidateRequest(request);

        var existing = await _usuarioRepository.GetByEmailAsync(request.Email, ct);
        if (existing is not null)
        {
            throw new DomainException("Email já cadastrado.");
        }

        var usuario = new Usuario(request.Nome, request.Email, request.Senha, request.AreaInteresse);
        await _usuarioRepository.AddAsync(usuario, ct);

        return new ResourceResponse<UsuarioResponse>(usuario.ToResponse(), BuildUsuarioLinks(usuario.Id));
    }

    public async Task<ResourceResponse<UsuarioResponse>> UpdateAsync(long id, UsuarioRequest request, CancellationToken ct = default)
    {
        ValidateRequest(request);

        var usuario = await _usuarioRepository.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Usuário não encontrado.");

        var existing = await _usuarioRepository.GetByEmailAsync(request.Email, ct);
        if (existing is not null && existing.Id != id)
        {
            throw new DomainException("Email já cadastrado.");
        }

        usuario.AtualizarPerfil(request.Nome, request.Email, request.AreaInteresse);
        usuario.AlterarSenha(request.Senha);
        await _usuarioRepository.UpdateAsync(usuario, ct);

        return new ResourceResponse<UsuarioResponse>(usuario.ToResponse(), BuildUsuarioLinks(id));
    }

    public async Task DeleteAsync(long id, CancellationToken ct = default)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id, ct);
        if (usuario is null)
        {
            throw new KeyNotFoundException("Usuário não encontrado.");
        }

        await _usuarioRepository.DeleteAsync(id, ct);
    }

    private static IQueryable<Usuario> ApplySort(IQueryable<Usuario> query, string? sortBy, string? direction)
    {
        var orderDesc = string.Equals(direction, "desc", StringComparison.OrdinalIgnoreCase);

        return sortBy?.ToLower() switch
        {
            "nome" => orderDesc ? query.OrderByDescending(u => u.Nome) : query.OrderBy(u => u.Nome),
            "email" => orderDesc ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
            "datacadastro" => orderDesc ? query.OrderByDescending(u => u.DataCadastro) : query.OrderBy(u => u.DataCadastro),
            _ => query.OrderBy(u => u.Id)
        };
    }

    private static void ValidateRequest(UsuarioRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome) || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Senha))
        {
            throw new DomainException("Nome, email e senha são obrigatórios.");
        }
    }

    private static IReadOnlyCollection<LinkResponse> BuildUsuarioLinks(long id)
    {
        return new[]
        {
            new LinkResponse("self", $"/api/usuarios/{id}"),
            new LinkResponse("update", $"/api/usuarios/{id}", "PUT"),
            new LinkResponse("delete", $"/api/usuarios/{id}", "DELETE"),
        };
    }
}
