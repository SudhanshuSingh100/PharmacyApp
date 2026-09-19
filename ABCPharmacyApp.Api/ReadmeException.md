Full workflow: Serilog + Correlation ID + ProblemDetails + File logs

1. Startup workflow
  dotnet run
   │
   ├─ 1. Bootstrap logger created (console only)
   │      └─ captures failures that happen before config is loaded
   ├─ 2. WebApplication.CreateBuilder loads configuration
   │      appsettings.json → appsettings.{Environment}.json → env vars → CLI args
   ├─ 3. UseSerilog(...) replaces the default logging providers
   │      └─ reads the "Serilog" section: levels, sinks, enrichers
   ├─ 4. DI registration
   │      services, correlation accessor, HttpClient handler,
   │      ProblemDetails + customization, GlobalExceptionHandler, mapper
   ├─ 5. Middleware pipeline built (order matters, see below)
   └─ 6. app.Run()  →  on shutdown: Log.CloseAndFlush()


   2. Request pipeline (order matters)

     Incoming request
      │
      ▼
┌──────────────────────────────────────────────────────────────┐
│ 1. CorrelationIdMiddleware                                   │
│    read X-Correlation-ID (validate) or generate a new one    │
│    → HttpContext.Items, response header, LogContext          │
├──────────────────────────────────────────────────────────────┤
│ 2. UseSerilogRequestLogging                                  │
│    times the request, writes one summary line at the end     │
│    (sees the final status code, including 500)               │
├──────────────────────────────────────────────────────────────┤
│ 3. UseExceptionHandler → GlobalExceptionHandler              │
│    catches anything thrown below                             │
├──────────────────────────────────────────────────────────────┤
│ 4. UseStatusCodePages                                        │
│    empty 401/403/404 → ProblemDetails                        │
├──────────────────────────────────────────────────────────────┤
│ 5. HttpsRedirection → Authentication → Authorization         │
├──────────────────────────────────────────────────────────────┤
│ 6. Controller → Service → (HttpClient → downstream)          │
└──────────────────────────────────────────────────────────────┘
Each layer only wraps the ones below it. That is why the correlation middleware is first: everything after it, including the exception handler and request logging, inherits CorrelationId.

Happy path: POST /api/medicine/42

Client ── X-Correlation-ID: abc123 ──►
  1. Middleware   : id = abc123, LogContext.Push(CorrelationId=abc123)
  2. Controller   : INF "HTTP POST /medicine/42 received"
  3. MedicineService : INF "Placing medicine 42 for Alice"
                    INF "Medicine 42 placed successfully"
  4. Request log  : INF "HTTP POST /api/medicine/42 responded 200 in 63 ms"
◄── 200 OK, X-Correlation-ID: abc123 ──
Every log line carries CorrelationId=abc123. Nothing in the service or controller had to pass it explicitly.

MedicineService throws NotFoundException("Medicine", 404)
      │  (no catch, no log in the service)
      ▼
GlobalExceptionHandler.TryHandleAsync
      │  1. client cancelled?      → log Information, return 499
      │  2. mapper.Map(exception)  → ProblemDetails (status, type, title, errorCode)
      │  3. log ONCE:
      │        status >= 500 → LogError  (with stack trace)
      │        status <  500 → LogWarning (no stack trace)
      │  4. set response status
      │  5. IProblemDetailsService.TryWriteAsync(...)
      ▼
CustomizeProblemDetails (EnrichProblemDetails)
      │  adds: instance, traceId, correlationId, errorCode (if missing)
      ▼
application/problem+json written to the client
      ▼
Request logging middleware logs "responded 404 in 12 ms"

----------------
{
  "type": "https://api.example.com/problems/not-found",
  "title": "Resource not found",
  "status": 404,
  "detail": "Medicine '404' was not found.",
  "instance": "POST /api/medicine/404",
  "errorCode": "resource_not_found",
  "traceId": "00-4bf92f...-00",
  "correlationId": "abc123"
}
------------------------------------

[10:31:12 INF] [abc123] Controllers.MedicineController | HTTP POST /orders/404 received
[10:31:12 INF] [abc123] Services.MedicineService | Placing order 404 for Guest
[10:31:12 WRN] [abc123] Middleware.GlobalExceptionHandler | Request POST /api/orders/404 failed -> 404 (resource_not_found): Medicine '404' was not found.
[10:31:12 INF] [abc123] Serilog.AspNetCore.RequestLoggingMiddleware | HTTP POST /api/orders/404 responded 404 in 12.4 ms

--------------------------------------

5. Non-exception errors
Source	Goes through	Result
Unknown route (404), 401, 403	UseStatusCodePages → IProblemDetailsService	Problem JSON + enrichment
[ApiController] model validation (400)	ProblemDetailsFactory	Problem JSON, correlationId included via CustomizeProblemDetails (verify with a bad body)
Exception thrown anywhere	GlobalExceptionHandler	Problem JSON + one log entry


All paths end at the same enrichment step, so clients always see the same shape.

-----------------------------------------

6. Where logs go

Serilog pipeline (config from appsettings)
   │
   ├─ Enrichers: FromLogContext (CorrelationId), MachineName, ThreadId, Application
   ├─ Level filter: Default=Information, Microsoft/System=Warning,
   │                ExceptionHandlerMiddleware=Fatal (avoid duplicate error)
   │
   ├─► Console sink → text in Development, JSON in Production/containers
   └─► File sink    → {AppContext.BaseDirectory}/logs/log-YYYYMMDD.json
                      daily rolling, 14 files retained


  For production aggregation, you can add a Seq, Elasticsearch, or Loki sink with one more WriteTo entry. The application code doesn't change.                    


  
ABCPharmacyApp.Api/

├── Controllers/
│   └── MedicineController.cs            → no try/catch, just call the service
├── Services/
│   ├── IMedicineService.cs / MedicineService.cs        → throws AppExceptions, no log-and-rethrow
│   ├── ISalesReportService.cs Z/ SalesReportService.cs      → ILoggerFactory demo
│   └── ICorrelationIdAccessor.cs                 → reads ID from HttpContext.Items
├── Exceptions/
│   └── AppExceptions.cs               → AppException + NotFound/Conflict/BusinessRule/Validation
├── Errors/
│   ├── ProblemTypes.cs                → type URLs + default titles
│   ├── IExceptionProblemMapper.cs
│   └── ExceptionProblemMapper.cs      → exception → ProblemDetails (single mapping table)
├── Middleware/
│   ├── CorrelationIdMiddleware.cs     → header → LogContext → response header
│   └── GlobalExceptionHandler.cs      → map + log once + write via IProblemDetailsService
├── Http/
│   └── CorrelationIdDelegatingHandler.cs         → forwards header to downstream calls
├── Extensions/
│   └── ProblemDetailsExtensions.cs    → EnrichProblemDetails (traceId, correlationId, ...)
├── Program.cs
├── appsettings.json
└── appsettings.Development.json



  AppExceptions:   Domain exceptions carry their own status and error code, so the mapper doesn't need a case per exception type. named it RequestValidationException to avoid clashing with System.ComponentModel.DataAnnotations.ValidationException.

  Only AppException messages reach clients. Any other exception type (NullReferenceException, SqlException, and so on) becomes a generic 500 with no message leaked.

  The handler decides only whether to log and at what level. The mapper decides what the client sees. The service and its customization decide the extra fields. Each concern has one home.

  Extensions/ProblemDetailsExtensions.cs

Enrichment that applies to every problem response:
UseStatusCodePages() is what makes a bare 404 or 401 (empty body) become a application/problem+json response with your enrichment. It uses IProblemDetailsService automatically once AddProblemDetails is registered.

In .NET 8, the framework's ExceptionHandlerMiddleware still writes its own "An unhandled exception has occurred" error line even when your IExceptionHandler handles the exception. Silence that category so only your handler logs:



How each one looks in your app

TraceId is created by ASP.NET Core with no code from you. The traceparent header has four parts:

traceparent: 00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01
             │  └──────────── TraceId ───────────┘ └── SpanId ──┘ └ flags
             version
TraceId (4bf92f...) stays the same across every service in the call chain.
SpanId (00f067...) changes at every hop or operation.


CorrelationId is the one we built. It's a plain string read from X-Correlation-ID or generated in CorrelationIdMiddleware, then pushed into Serilog's LogContext.

Example: same request, both IDs
Client sends:   X-Correlation-ID: order-support-771
                (no traceparent)

Your API creates:  TraceId = 4bf92f3577b34da6a3ce929d0e0e4736   (automatic)
Your API uses:     CorrelationId = order-support-771            (from the header)

The client retries the failed request three times with the same X-Correlation-ID:

Attempt	CorrelationId	TraceId
1	order-support-771	aaa111...
2	order-support-771	bbb222...
3	order-support-771	ccc333...
TraceId identifies one attempt.
CorrelationId identifies the whole user action, across all attempts.

That's the practical difference: a trace is technical and per operation, while a correlation ID is logical and can span several traces.

Can you use just one?
    Yes, if you prefer simplicity:
    Microservices with OpenTelemetry: rely on TraceId and drop the custom header. Return the TraceId to clients in error responses (we already do).
    A small monolith or simple API: CorrelationId alone is enough.
    Unify them: have the middleware set CorrelationId = Activity.Current?.TraceId.ToString() when no X-Correlation-ID header is supplied. You then get one value that works in both worlds.