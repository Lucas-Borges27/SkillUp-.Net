using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SkillUp.Infrastructure;

public class SkillUpDbContextFactory : IDesignTimeDbContextFactory<SkillUpDbContext>
{
    public SkillUpDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<SkillUpDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                               Environment.GetEnvironmentVariable("SKILLUP_CONNECTION") ??
                               "User Id=skillup;Password=skillup;Data Source=localhost/XEPDB1";

        optionsBuilder.UseOracle(connectionString);
        return new SkillUpDbContext(optionsBuilder.Options);
    }
}
