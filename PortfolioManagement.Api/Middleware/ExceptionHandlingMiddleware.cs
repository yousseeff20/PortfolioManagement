using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace PortfolioManagement.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException exception)
        {
            if (context.Response.HasStarted)
            {
                _logger.LogWarning("Response already started, cannot write validation error");
                return;
            }

            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsJsonAsync(new ValidationProblemDetails(
                exception.Errors.GroupBy(e => e.PropertyName).ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()))
            {
                Title = "Validation failed",
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception");

            if (context.Response.HasStarted)
            {
                _logger.LogWarning("Response already started, cannot write error details");
                return;
            }

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Title = "An unexpected error occurred",
                Detail = context.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment() ? exception.Message : null,
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }
}
