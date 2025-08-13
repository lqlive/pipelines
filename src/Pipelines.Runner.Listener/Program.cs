using Pipelines.Core.Scheduling;
using Pipelines.Runner.Listener;
using Pipelines.Runner.Listener.Apis;
using Pipelines.Runner.Listener.Configuration;
using Pipelines.Runner.Listener.JobDispatcher;
using Pipelines.Runner.Listener.RunnerManagement;
using Pipelines.Runner.Listener.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);
// Enable HTTP/2 for gRPC (allow h2c for development)
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5170, o =>
    {
        o.Protocols = HttpProtocols.Http1AndHttp2;
    });
});

// Configuration
builder.Services.Configure<ListenerConfiguration>(
    builder.Configuration.GetSection(ListenerConfiguration.SectionName));

// Add services
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();
builder.Services.AddGrpc();

// Job dispatching system
builder.Services.AddSingleton<IJobQueue, InMemoryJobQueue>();
builder.Services.AddSingleton<IJobScheduler, JobScheduler>();

// Runner management system
builder.Services.AddSingleton<IRunnerRegistry, InMemoryRunnerRegistry>();
builder.Services.AddSingleton<Pipelines.Services.Builds.LogStorageService>();

// Background services - following GitHub Runner.Listener pattern
builder.Services.AddHostedService<MessageListener>();
builder.Services.AddHostedService<RunnerCleanupService>();

// CORS for cross-origin requests
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                "https://localhost:5169", "http://localhost:5169",  // Pipelines API
                "https://localhost:3000", "http://localhost:3000",  // React UI
                "https://localhost:5173", "http://localhost:5173"   // Vite dev
              )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();
app.MapHealthChecks("/health");
app.MapGrpcService<GrpcRunnerService>();
app.MapGrpcService<GrpcLogService>();

// Map APIs
app.MapSchedulerApi();
app.MapRunnerApi();

// Startup banner - GitHub Runner style
Console.WriteLine("╔═══════════════════════════════════════════╗");
Console.WriteLine("║    🎧 Pipelines Runner Listener v1.0     ║");
Console.WriteLine("╚═══════════════════════════════════════════╝");
Console.WriteLine($"🚀 Listening on: {app.Urls.FirstOrDefault() ?? "http://localhost:5170"}");
Console.WriteLine("📋 Available endpoints:");
Console.WriteLine("  • Health: GET /health");
Console.WriteLine("  • Job Scheduling: POST /api/scheduler/jobs");
Console.WriteLine("  • Job Control: DELETE /api/scheduler/jobs/{id}");
Console.WriteLine("  • Queue Stats: GET /api/scheduler/stats");
Console.WriteLine("  • Runner Registration: POST /api/runners/register");
Console.WriteLine("  • Runner Heartbeat: POST /api/runners/{id}/heartbeat");
Console.WriteLine("  • Job Request: POST /api/runners/{id}/jobs/request");
Console.WriteLine("");

app.Run();
