using ABCPharmacyApp.Api.Middleware;


namespace ABCPharmacyApp.Api.Services.IServices;
public interface ICorrelationIdAccessor
{
    string? CorrelationId { get; }
}