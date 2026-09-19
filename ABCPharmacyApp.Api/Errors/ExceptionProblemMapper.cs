using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using  ABCPharmacyApp.Api.Exceptions;

namespace ABCPharmacyApp.Api.Errors;

public sealed class ExceptionProblemMapper(IHostEnvironment environment) : IExceptionProblemMapper
{
    public ProblemDetails Map(HttpContext httpContext, Exception exception)
    {
        ProblemDetails problem = exception switch
        {
            RequestValidationException v => new HttpValidationProblemDetails(v.Errors)
            {
                Status = v.StatusCode,
                Type   = ProblemTypes.Validation,
                Title  = "One or more validation errors occurred.",
                Detail = "See the 'errors' property for details."
            },

            AppException app => Create(app.StatusCode, app.Message),

            BadHttpRequestException bad => Create(StatusCodes.Status400BadRequest, bad.Message),

            _ => Create(StatusCodes.Status500InternalServerError, detail: null)
        };

        // Stable machine-readable code for clients
        problem.Extensions["errorCode"] = exception switch
        {
            AppException app          => app.ErrorCode,
            BadHttpRequestException   => "bad_request",
            _                         => "internal_error"
        };

        // Development only: help debugging, never expose in production
        if (environment.IsDevelopment() && problem.Status >= 500)
        {
            problem.Detail = exception.Message;
            problem.Extensions["exception"] = exception.GetType().FullName;
            problem.Extensions["stackTrace"] = exception.StackTrace;
        }

        return problem;
    }

    private static ProblemDetails Create(int status, string? detail)
    {
        var (type, title) = ProblemTypes.ForStatus(status);

        return new ProblemDetails
        {
            Status = status,
            Type   = type,
            Title  = title,
            // 5xx: never leak internals. 4xx: messages come from our own exceptions.
            Detail = status >= 500
                ? "An unexpected error occurred. Please contact support and quote the correlation ID."
                : detail
        };
    }
}