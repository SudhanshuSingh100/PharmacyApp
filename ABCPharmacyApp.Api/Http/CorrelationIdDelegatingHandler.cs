using ABCPharmacyApp.Api.Middleware;
using ABCPharmacyApp.Api.Services;
using ABCPharmacyApp.Api.Services.IServices;

namespace ABCPharmacyApp.Api.Http;

public class CorrelationIdDelegatingHandler : DelegatingHandler
{
    private readonly ICorrelationIdAccessor _accessor;

    public CorrelationIdDelegatingHandler(ICorrelationIdAccessor accessor)
        => _accessor = accessor;

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var id = _accessor.CorrelationId;

        if (!string.IsNullOrEmpty(id) &&
            !request.Headers.Contains(CorrelationIdMiddleware.HeaderName))
        {
            request.Headers.Add(CorrelationIdMiddleware.HeaderName, id);
        }

        return base.SendAsync(request, cancellationToken);
    }
}