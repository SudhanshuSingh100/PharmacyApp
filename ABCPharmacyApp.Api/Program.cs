using ABCPharmacyApp.Api.Services;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;



var builder = WebApplication.CreateBuilder(args);



// Register global exception handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();



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

builder.Services.AddSingleton<JsonStorageService>();
builder.Services.AddScoped<MedicineService>();
// Add services to the container.
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowSpa");
app.MapControllers();
//  Expose /metrics endpoint for Prometheus
//app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.Run();

// This project design and developed by Sudhanshu Singh 
// as a showcase to Publicis Sapient of .NET Software buliding skill, 
// either end to end REST API developement and frontend, 
// that shows hand-on experience, Knowledge of system design,
//  From HLD to LLD, coding style and leveraging the AI tools.

