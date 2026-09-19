using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;
using ABCPharmacyApp.Api.Errors;
using ABCPharmacyApp.Api.Exceptions;

namespace ABCPharmacyApp.Api.Middleware;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IExceptionProblemMapper mapper,
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        // Client disconnected: not an error, and no one is listening for a response.
        if (exception is OperationCanceledException && httpContext.RequestAborted.IsCancellationRequested)
        {
            logger.LogInformation("Request {Method} {Path} was cancelled by the client",
                httpContext.Request.Method, httpContext.Request.Path);
            httpContext.Response.StatusCode = 499;
            return true;
        }

        var problem = mapper.Map(httpContext, exception);
        var status = problem.Status ?? StatusCodes.Status500InternalServerError;
        var errorCode = problem.Extensions.TryGetValue("errorCode", out var code) ? code : null;

        if (status >= 500)
        {
            // Unexpected failure: full exception with stack trace
            logger.LogError(exception,
                "Unhandled exception on {Method} {Path} -> {StatusCode} ({ErrorCode})",
                httpContext.Request.Method, httpContext.Request.Path, status, errorCode);
        }
        else
        {
            // Expected/client error: no stack trace, no noise
            logger.LogWarning(
                "Request {Method} {Path} failed -> {StatusCode} ({ErrorCode}): {Reason}",
                httpContext.Request.Method, httpContext.Request.Path, status, errorCode, exception.Message);
        }

        httpContext.Response.StatusCode = status;

        // IProblemDetailsService runs CustomizeProblemDetails, adding correlationId, traceId, instance
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problem
        });
    }
}