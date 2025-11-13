using SkillUp.Application.DTOs.Common;
using SkillUp.Application.DTOs.Progresso;

namespace SkillUp.Application.Interfaces;

public interface IProgressoAppService
{
    Task<PagedResponse<ProgressoResponse>> SearchAsync(ProgressoSearchQuery query, CancellationToken ct = default);
    Task<ResourceResponse<ProgressoResponse>> GetByIdAsync(long id, CancellationToken ct = default);
    Task<ResourceResponse<ProgressoResponse>> CreateAsync(ProgressoRequest request, CancellationToken ct = default);
    Task<ResourceResponse<ProgressoResponse>> UpdateAsync(long id, ProgressoRequest request, CancellationToken ct = default);
    Task DeleteAsync(long id, CancellationToken ct = default);
}
