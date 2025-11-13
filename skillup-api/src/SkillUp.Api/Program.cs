using Microsoft.AspNetCore.Mvc;
using SkillUp.Application;
using SkillUp.Infrastructure;
using SkillUp.Infrastructure.Seed;
using SkillUp.Api.Middlewares;
using SkillUp.Api.Extensions;
using Microsoft.AspNetCore.Mvc.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var useInMemoryDatabase = builder.Configuration.GetValue<bool>("Database:UseInMemory");

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration, useInMemoryDatabase);
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value!.Errors.Select(error => error.ErrorMessage).ToArray());

        var problemDetails = new ValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Falha de validação",
            Detail = "Verifique os campos enviados",
            Instance = context.HttpContext.Request.Path
        };

        return new BadRequestObjectResult(problemDetails);
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ProblemDetailsFactory, CustomProblemDetailsFactory>();

var app = builder.Build();

if (useInMemoryDatabase)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<SkillUpDbContext>();
    await SkillUpDbSeeder.SeedAsync(context);
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();
