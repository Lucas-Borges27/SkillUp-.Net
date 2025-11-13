using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using SkillUp.Domain.Exceptions;

namespace SkillUp.Api.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly ProblemDetailsFactory _problemDetailsFactory;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, ProblemDetailsFactory problemDetailsFactory)
    {
        _next = next;
        _logger = logger;
        _problemDetailsFactory = problemDetailsFactory;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DomainException ex)
        {
            await WriteProblemAsync(context, StatusCodes.Status400BadRequest, ex.Message, "DomainViolation", ex);
        }
        catch (KeyNotFoundException ex)
        {
            await WriteProblemAsync(context, StatusCodes.Status404NotFound, ex.Message, "NotFound", ex);
        }
        catch (ArgumentException ex)
        {
            await WriteProblemAsync(context, StatusCodes.Status400BadRequest, ex.Message, "ArgumentError", ex);
        }
        catch (InvalidOperationException ex)
        {
            await WriteProblemAsync(context, StatusCodes.Status409Conflict, ex.Message, "InvalidOperation", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado");
            await WriteProblemAsync(context, StatusCodes.Status500InternalServerError, "Erro inesperado", "ServerError", ex);
        }
    }

    private async Task WriteProblemAsync(HttpContext context, int statusCode, string detail, string title, Exception exception)
    {
        var problem = _problemDetailsFactory.CreateProblemDetails(context, statusCode, title: title, detail: detail);
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problem);
    }
}
