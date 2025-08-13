using Pipelines.Core.Entities.Builds;
using Pipelines.Runner.Docker.Host;
using Pipelines.Runner.Docker.Worker;
using Pipelines.Runner.Docker.Client;
using Pipelines.Runner.Docker.Services;
using Pipelines.Core.Runner;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
var builder = Host.CreateApplicationBuilder(args);

// No HTTP clients needed when using gRPC

// Runner configuration
builder.Services.AddSingleton(new RunnerConfiguration
{
    Name = Environment.GetEnvironmentVariable("RUNNER_NAME") ?? Environment.MachineName,
    Capabilities = Environment.GetEnvironmentVariable("RUNNER_CAPABILITIES")?.Split(',') ?? ["docker", "linux", "ubuntu"],
    MaxConcurrentJobs = int.TryParse(Environment.GetEnvironmentVariable("RUNNER_MAX_JOBS"), out var maxJobs) ? maxJobs : 1,
    Version = "1.0.0"
});

// Services
builder.Services.AddSingleton<StepRunner>();
builder.Services.AddSingleton<JobRunner>();
builder.Services.AddSingleton<RunnerRegistrationService>();
builder.Services.AddHostedService<RunnerRegistrationService>();
builder.Services.AddHostedService<RunnerService>();
builder.Services.AddSingleton<IJobServer, HttpJobServer>();
builder.Services.AddSingleton<IJobServerQueue, GrpcJobServerQueue>();

var app = builder.Build();
await app.RunAsync();


