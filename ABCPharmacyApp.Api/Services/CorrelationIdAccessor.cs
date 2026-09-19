using ABCPharmacyApp.Api.Middleware;
using ABCPharmacyApp.Api.Services.IServices;
namespace ABCPharmacyApp.Api.Services;

public class CorrelationIdAccessor : ICorrelationIdAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CorrelationIdAccessor(IHttpContextAccessor httpContextAccessor)
        => _httpContextAccessor = httpContextAccessor;

    public string? CorrelationId =>
        _httpContextAccessor.HttpContext?.Items[CorrelationIdMiddleware.HeaderName] as string;
}