using ABCPharmacyApp.Api.Services;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using Serilog;
using ABCPharmacyApp.Api.Http;
using ABCPharmacyApp.Api.Middleware;
using ABCPharmacyApp.Api.Services.IServices;
using ABCPharmacyApp.Api.Extensions;
using ABCPharmacyApp.Api.Exceptions;
using ABCPharmacyApp.Api.Errors;


Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting web application");

    var builder = WebApplication.CreateBuilder(args);
    
    // ---- Logging ----
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

  

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowSpa", policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
            // policy.WithOrigins("http://localhost:5173")
            //       .AllowAnyHeader()
            //       .AllowAnyMethod();
        });
    });

       // Configure OpenTelemetry
    builder.Services.AddOpenTelemetry()
        .ConfigureResource(resource => resource.AddService("ABCPharmacyApp.Api"))
        .WithTracing(tracerProviderBuilder => tracerProviderBuilder
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddConsoleExporter()   // Logs traces to console
            .AddOtlpExporter(opt =>
            {
                // Works with OpenTelemetry Collector, Jaeger, etc.
            // opt.Endpoint = new Uri("http://localhost:4317"); // gRPC default
                opt.Endpoint = new Uri("http://localhost:4318/v1/traces");  // For HTTP use
            }))
        .WithMetrics(metricsProviderBuilder => metricsProviderBuilder
            .AddAspNetCoreInstrumentation()
            .AddRuntimeInstrumentation()
            .AddConsoleExporter());
        // .AddPrometheusExporter());   // <-- Prometheus exporter

    builder.Services.AddControllers();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        // Include XML comments
        var xmlFile = "ABCPharmacyApp.Api.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);
    });
    // Domain services
    builder.Services.AddSingleton<JsonStorageService>();
    builder.Services.AddScoped<MedicineService>();
    // Add services to the container.

     // Correlation ID support
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<ICorrelationIdAccessor, CorrelationIdAccessor>();
    builder.Services.AddTransient<CorrelationIdDelegatingHandler>();
    //  typed/named client that forwards the header downstream
    builder.Services.AddHttpClient("downstream", client =>
        {
            client.BaseAddress = new Uri("https://example.com/");
        })
        .AddHttpMessageHandler<CorrelationIdDelegatingHandler>();

    //Problem details pipeline : ProblemDetails + add correlationId to EVERY problem response pipeline
    builder.Services.AddSingleton<IExceptionProblemMapper, ExceptionProblemMapper>();
    builder.Services.AddProblemDetails(options =>
        options.CustomizeProblemDetails = ProblemDetailsExtensions.EnrichProblemDetails);
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    
    // ORDER MATTERS: correlation middleware must come BEFORE request logging,
    // so the "HTTP POST ... responded 200" line also has the CorrelationId.
    // ORDER MATTERS
    app.UseMiddleware<CorrelationIdMiddleware>();   // 1. set the ID + LogContext
    app.UseSerilogRequestLogging();                 // 2. sees the final status (e.g. 500)
    app.UseExceptionHandler();                      // 3. converts exceptions to ProblemDetails
    app.UseStatusCodePages();

    app.UseHttpsRedirection();
    app.UseCors("AllowSpa");
    app.UseAuthentication();   // add if you use auth
    app.UseAuthorization();
    app.MapControllers();
    //  Expose /metrics endpoint for Prometheus
    //app.UseOpenTelemetryPrometheusScrapingEndpoint();

    app.Run();
}
catch(Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Appliaction terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}


// This project design and developed by Sudhanshu Singh 
// as a showcase to Publicis Sapient of .NET Software buliding skill, 
// either end to end REST API developement and frontend, 
// that shows hand-on experience, Knowledge of system design,
//  From HLD to LLD, coding style and leveraging the AI tools.

