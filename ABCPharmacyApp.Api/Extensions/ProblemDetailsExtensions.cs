
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using ABCPharmacyApp.Api.Errors;
using ABCPharmacyApp.Api.Middleware;

namespace ABCPharmacyApp.Api.Extensions;

public static class ProblemDetailsExtensions
{
    public static void EnrichProblemDetails(ProblemDetailsContext context)
    {
        var http = context.HttpContext;
        var problem = context.ProblemDetails;

        // Fill type/title for responses that did not come from our mapper
        // (401, 403, 404 from status code pages, framework 400s, ...)
        if (problem.Status is int status)
        {
            var (type, title) = ProblemTypes.ForStatus(status);
            problem.Type  ??= type;
            problem.Title ??= title;
        }

        problem.Instance ??= $"{http.Request.Method} {http.Request.Path}";

        problem.Extensions["traceId"] = Activity.Current?.Id ?? http.TraceIdentifier;

        if (http.Items[CorrelationIdMiddleware.HeaderName] is string correlationId)
            problem.Extensions["correlationId"] = correlationId;

        // Every problem response has an errorCode, even framework-generated ones
        if (!problem.Extensions.ContainsKey("errorCode") && problem.Status is int s)
        {
            problem.Extensions["errorCode"] = s switch
            {
                401 => "unauthorized",
                403 => "forbidden",
                404 => "not_found",
                _   => "request_failed"
            };
        }
    }
}