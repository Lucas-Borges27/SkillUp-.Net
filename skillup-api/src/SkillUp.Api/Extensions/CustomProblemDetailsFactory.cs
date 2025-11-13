using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SkillUp.Api.Extensions;

public class CustomProblemDetailsFactory : ProblemDetailsFactory
{
    public override ProblemDetails CreateProblemDetails(HttpContext httpContext, int? statusCode = null, string? title = null,
        string? type = null, string? detail = null, string? instance = null)
    {
        statusCode ??= StatusCodes.Status500InternalServerError;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Type = type,
            Detail = detail,
            Instance = instance ?? httpContext.Request.Path
        };

        ApplyExtensions(httpContext, problemDetails);
        return problemDetails;
    }

    public override ValidationProblemDetails CreateValidationProblemDetails(HttpContext httpContext, ModelStateDictionary modelStateDictionary,
        int? statusCode = null, string? title = null, string? type = null, string? detail = null, string? instance = null)
    {
        if (modelStateDictionary == null)
        {
            throw new ArgumentNullException(nameof(modelStateDictionary));
        }

        statusCode ??= StatusCodes.Status400BadRequest;

        var problemDetails = new ValidationProblemDetails(modelStateDictionary)
        {
            Status = statusCode,
            Type = type,
            Detail = detail,
            Instance = instance ?? httpContext.Request.Path,
            Title = title ?? "Falha de validação"
        };

        ApplyExtensions(httpContext, problemDetails);
        return problemDetails;
    }

    private static void ApplyExtensions(HttpContext httpContext, ProblemDetails problemDetails)
    {
        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
    }
}
