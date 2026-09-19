using Microsoft.AspNetCore.Mvc;

namespace ABCPharmacyApp.Api.Errors;

public interface IExceptionProblemMapper
{
    ProblemDetails Map(HttpContext httpContext, Exception exception);
}