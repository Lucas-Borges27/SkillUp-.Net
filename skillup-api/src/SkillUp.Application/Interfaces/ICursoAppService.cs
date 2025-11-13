using SkillUp.Application.DTOs.Common;
using SkillUp.Application.DTOs.Cursos;

namespace SkillUp.Application.Interfaces;

public interface ICursoAppService
{
    Task<PagedResponse<CursoResponse>> SearchAsync(CursoSearchQuery query, CancellationToken ct = default);
    Task<ResourceResponse<CursoResponse>> GetByIdAsync(long id, CancellationToken ct = default);
    Task<ResourceResponse<CursoResponse>> CreateAsync(CursoRequest request, CancellationToken ct = default);
    Task<ResourceResponse<CursoResponse>> UpdateAsync(long id, CursoRequest request, CancellationToken ct = default);
    Task DeleteAsync(long id, CancellationToken ct = default);
}
