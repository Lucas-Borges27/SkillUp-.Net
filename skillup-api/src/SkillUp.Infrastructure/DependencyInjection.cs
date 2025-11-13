using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkillUp.Domain.Repositories;
using SkillUp.Infrastructure.Repositories;

namespace SkillUp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, bool useInMemory = false)
    {
        if (useInMemory)
        {
            services.AddDbContext<SkillUpDbContext>(options => options.UseInMemoryDatabase("SkillUpDb"));
        }
        else
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                                   Environment.GetEnvironmentVariable("SKILLUP_CONNECTION") ??
                                   "User Id=skillup;Password=skillup;Data Source=localhost/XEPDB1";

            services.AddDbContext<SkillUpDbContext>(options => options.UseOracle(connectionString));
        }

        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<ICursoRepository, CursoRepository>();
        services.AddScoped<IProgressoRepository, ProgressoRepository>();

        return services;
    }
}
