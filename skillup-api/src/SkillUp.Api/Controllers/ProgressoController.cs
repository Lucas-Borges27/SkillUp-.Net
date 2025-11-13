using Microsoft.AspNetCore.Mvc;
using SkillUp.Application.DTOs.Common;
using SkillUp.Application.DTOs.Progresso;
using SkillUp.Application.Interfaces;

namespace SkillUp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProgressoController : ControllerBase
{
    private readonly IProgressoAppService _progressoAppService;

    public ProgressoController(IProgressoAppService progressoAppService)
    {
        _progressoAppService = progressoAppService;
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(PagedResponse<ProgressoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchAsync([FromQuery] ProgressoSearchQuery query, CancellationToken ct)
    {
        var result = await _progressoAppService.SearchAsync(query, ct);
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ResourceResponse<ProgressoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(long id, CancellationToken ct)
    {
        var result = await _progressoAppService.GetByIdAsync(id, ct);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ResourceResponse<ProgressoResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAsync([FromBody] ProgressoRequest request, CancellationToken ct)
    {
        var result = await _progressoAppService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = result.Data.Id }, result);
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(ResourceResponse<ProgressoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateAsync(long id, [FromBody] ProgressoRequest request, CancellationToken ct)
    {
        var result = await _progressoAppService.UpdateAsync(id, request, ct);
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync(long id, CancellationToken ct)
    {
        await _progressoAppService.DeleteAsync(id, ct);
        return NoContent();
    }
}
