using SkillUp.Application.DTOs.Common;
using SkillUp.Application.DTOs.Usuarios;

namespace SkillUp.Application.Interfaces;

public interface IUsuarioAppService
{
    Task<PagedResponse<UsuarioResponse>> SearchAsync(UsuarioSearchQuery query, CancellationToken ct = default);
    Task<ResourceResponse<UsuarioResponse>> GetByIdAsync(long id, CancellationToken ct = default);
    Task<ResourceResponse<UsuarioResponse>> CreateAsync(UsuarioRequest request, CancellationToken ct = default);
    Task<ResourceResponse<UsuarioResponse>> UpdateAsync(long id, UsuarioRequest request, CancellationToken ct = default);
    Task DeleteAsync(long id, CancellationToken ct = default);
}
