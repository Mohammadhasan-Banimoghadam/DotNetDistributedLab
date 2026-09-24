using OrderService.Application.Clients;
using OrderService.Application.Services;
using OrderService.Infrastructure.Middleware;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using OpenTelemetry.Logs;
using Microsoft.EntityFrameworkCore;
using OrderService.Infrastructure.Persistence;
//using Serilog;
//using Serilog.Enrichers.Span;

//Log.Logger = new LoggerConfiguration()
//    .MinimumLevel.Information()
//    .Enrich.FromLogContext()
//    .Enrich.WithSpan()
//    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("OrderDb")));

var otlpEndpoint =
    builder.Configuration["OpenTelemetry:OtlpEndpoint"]
    ?? throw new InvalidOperationException("OpenTelemetry endpoint is not configured.");

var customerServiceUrl =
    builder.Configuration["Services:CustomerService"]
    ?? throw new InvalidOperationException("CustomerService URL is not configured.");

var paymentServiceUrl =
    builder.Configuration["Services:PaymentService"]
    ?? throw new InvalidOperationException("PaymentService URL is not configured.");

//builder.Host.UseSerilog();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeFormattedMessage = true;
    logging.IncludeScopes = true;

    logging.AddOtlpExporter(options =>
    {
        options.Endpoint = new Uri(otlpEndpoint);
    });
});

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource =>
        resource.AddService("OrderService"))

    // Traces
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(otlpEndpoint);
            });
    })

    // Metrics
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(otlpEndpoint);
            });
    });

builder.Services
    .AddHttpClient<CustomerClient>(client =>
    {
        client.BaseAddress = new Uri(customerServiceUrl);
    })
    .AddStandardResilienceHandler(options =>
        {
            options.Retry.MaxRetryAttempts = 3;

            options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(30);

            options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);

            options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);

            options.CircuitBreaker.FailureRatio = 0.5;

            options.CircuitBreaker.MinimumThroughput = 2;
        });


builder.Services
    .AddHttpClient<PaymentClient>(client =>
    {
        client.BaseAddress = new Uri(paymentServiceUrl);
    })
    .AddStandardResilienceHandler(options =>
    {
        options.Retry.MaxRetryAttempts = 3;

        options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(30);

        options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);

        options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);

        options.CircuitBreaker.FailureRatio = 0.5;

        options.CircuitBreaker.MinimumThroughput = 2;
    });


builder.Services.AddScoped<OrderApplicationService>();
builder.Services.AddControllers();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();



