using Microsoft.AspNetCore.Mvc;
using SkillUp.Application.DTOs.Common;
using SkillUp.Application.DTOs.Cursos;
using SkillUp.Application.Interfaces;

namespace SkillUp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CursosController : ControllerBase
{
    private readonly ICursoAppService _cursoAppService;

    public CursosController(ICursoAppService cursoAppService)
    {
        _cursoAppService = cursoAppService;
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(PagedResponse<CursoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchAsync([FromQuery] CursoSearchQuery query, CancellationToken ct)
    {
        var result = await _cursoAppService.SearchAsync(query, ct);
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ResourceResponse<CursoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(long id, CancellationToken ct)
    {
        var result = await _cursoAppService.GetByIdAsync(id, ct);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ResourceResponse<CursoResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAsync([FromBody] CursoRequest request, CancellationToken ct)
    {
        var result = await _cursoAppService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = result.Data.Id }, result);
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(ResourceResponse<CursoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateAsync(long id, [FromBody] CursoRequest request, CancellationToken ct)
    {
        var result = await _cursoAppService.UpdateAsync(id, request, ct);
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync(long id, CancellationToken ct)
    {
        await _cursoAppService.DeleteAsync(id, ct);
        return NoContent();
    }
}
