using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nunari.Auth.Application.Exceptions;

namespace Nunari.Auth.Infrastructure;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // 1. Log the error centrally
        logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

        // 2. Map specific exceptions to status codes (Optional)
        var statusCode = exception switch
        {
            ArgumentException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            EmailAlreadyExistException => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        // 3. Create a standardized Problem Details response
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = "An error occurred",
            Detail = exception.Message,
            Instance = httpContext.Request.Path
        };

        // 4. Write the response
        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true; // The exception has been handled
    }
}
