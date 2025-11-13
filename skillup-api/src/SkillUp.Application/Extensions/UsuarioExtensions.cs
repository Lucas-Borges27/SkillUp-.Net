using SkillUp.Application.DTOs.Usuarios;
using SkillUp.Domain.Entities;

namespace SkillUp.Application.Extensions;

public static class UsuarioExtensions
{
    public static UsuarioResponse ToResponse(this Usuario usuario) =>
        new(usuario.Id, usuario.Nome, usuario.Email, usuario.AreaInteresse, usuario.DataCadastro);
}
