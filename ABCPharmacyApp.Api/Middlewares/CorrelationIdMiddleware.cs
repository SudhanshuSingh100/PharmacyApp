using Serilog.Context;

namespace ABCPharmacyApp.Api.Middleware;

public class CorrelationIdMiddleware
{
    public const string HeaderName = "X-Correlation-ID";
    public const string PropertyName = "CorrelationId";

    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetOrCreateCorrelationId(context);

        // Make it available to other code in this request (e.g. HttpClient handler)
        context.Items[HeaderName] = correlationId;

        // Echo it back to the caller
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = correlationId;
            return Task.CompletedTask;
        });

        // Every Serilog event inside this block gets {CorrelationId}
        using (LogContext.PushProperty(PropertyName, correlationId))
        {
            await _next(context);
        }
    }

    private static string GetOrCreateCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(HeaderName, out var values))
        {
            var incoming = values.FirstOrDefault();
            if (IsValid(incoming))
                return incoming!;
        }

        return Guid.NewGuid().ToString("N");
    }

    // Never trust client input blindly: limit length and characters
    // to avoid log injection / oversized values.
    private static bool IsValid(string? value) =>
        !string.IsNullOrWhiteSpace(value)
        && value.Length <= 64
        && value.All(c => char.IsLetterOrDigit(c) || c is '-' or '_');
}