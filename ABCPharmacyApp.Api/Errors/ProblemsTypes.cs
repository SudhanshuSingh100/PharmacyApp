namespace ABCPharmacyApp.Api.Errors;

public static class ProblemTypes
{
    // Use URLs you control. They can point to real docs pages later.
    private const string Base = "https://api.example.com/problems/";

    public const string Validation   = Base + "validation-failed";
    public const string NotFound     = Base + "not-found";
    public const string Conflict     = Base + "conflict";
    public const string BusinessRule = Base + "business-rule";
    public const string BadRequest   = Base + "bad-request";
    public const string Unauthorized = Base + "unauthorized";
    public const string Forbidden    = Base + "forbidden";
    public const string Internal     = Base + "internal-error";

    public static (string Type, string Title) ForStatus(int status) => status switch
    {
        400 => (BadRequest,   "Bad request"),
        401 => (Unauthorized, "Authentication required"),
        403 => (Forbidden,    "Access denied"),
        404 => (NotFound,     "Resource not found"),
        409 => (Conflict,     "Conflict"),
        422 => (BusinessRule, "Business rule violation"),
        >= 500 => (Internal,  "An unexpected error occurred"),
        _   => (BadRequest,   "Request failed")
    };
}