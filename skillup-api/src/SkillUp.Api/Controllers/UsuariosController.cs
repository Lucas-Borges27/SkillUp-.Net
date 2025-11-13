using Microsoft.AspNetCore.Mvc;
using SkillUp.Application.DTOs.Common;
using SkillUp.Application.DTOs.Usuarios;
using SkillUp.Application.Interfaces;

namespace SkillUp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioAppService _usuarioAppService;

    public UsuariosController(IUsuarioAppService usuarioAppService)
    {
        _usuarioAppService = usuarioAppService;
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(PagedResponse<UsuarioResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchAsync([FromQuery] UsuarioSearchQuery query, CancellationToken ct)
    {
        var result = await _usuarioAppService.SearchAsync(query, ct);
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ResourceResponse<UsuarioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(long id, CancellationToken ct)
    {
        var result = await _usuarioAppService.GetByIdAsync(id, ct);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ResourceResponse<UsuarioResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAsync([FromBody] UsuarioRequest request, CancellationToken ct)
    {
        var result = await _usuarioAppService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = result.Data.Id }, result);
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(ResourceResponse<UsuarioResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateAsync(long id, [FromBody] UsuarioRequest request, CancellationToken ct)
    {
        var result = await _usuarioAppService.UpdateAsync(id, request, ct);
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync(long id, CancellationToken ct)
    {
        await _usuarioAppService.DeleteAsync(id, ct);
        return NoContent();
    }
}
