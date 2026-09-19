namespace ABCPharmacyApp.Api.Exceptions;

public abstract class AppException : Exception
{
    protected AppException(string message, int statusCode, string errorCode, Exception? inner = null)
        : base(message, inner)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }

    public int StatusCode { get; }

    /// <summary>Stable, machine-readable code clients can switch on.</summary>
    public string ErrorCode { get; }
}

public sealed class NotFoundException(string resource, object key)
    : AppException($"{resource} '{key}' was not found.", StatusCodes.Status404NotFound, "resource_not_found");

public sealed class ConflictException(string message)
    : AppException(message, StatusCodes.Status409Conflict, "conflict");

public sealed class BusinessRuleException(string message, string errorCode = "business_rule_violation")
    : AppException(message, StatusCodes.Status422UnprocessableEntity, errorCode);

public sealed class RequestValidationException : AppException
{
    public RequestValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.", StatusCodes.Status400BadRequest, "validation_failed")
        => Errors = errors;

    public RequestValidationException(string field, string error)
        : this(new Dictionary<string, string[]> { [field] = [error] }) { }

    public IDictionary<string, string[]> Errors { get; }
}