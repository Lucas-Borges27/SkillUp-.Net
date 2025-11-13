using Microsoft.Extensions.DependencyInjection;
using SkillUp.Application.Interfaces;
using SkillUp.Application.Services;

namespace SkillUp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUsuarioAppService, UsuarioAppService>();
        services.AddScoped<ICursoAppService, CursoAppService>();
        services.AddScoped<IProgressoAppService, ProgressoAppService>();
        return services;
    }
}
