using Docker.DotNet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pipelines.Core.Abstractions;
using Pipelines.Core.Models;
using Pipelines.Runner.Docker;
using Pipelines.Runner.Docker.Container;
using Pipelines.Runner.Listener;
using Pipelines.Runner.Worker;
using System.Runtime.InteropServices;


var builder = Host.CreateApplicationBuilder(args);
var runnerId = Guid.NewGuid();
var serverUrl = "http://localhost:5011";

builder.Services.AddSingleton(new RunnerProfile
{
    RunnerId = runnerId,
    Type = "docker",
    Os = RuntimeInformation.OSDescription,
    Architecture = RuntimeInformation.OSArchitecture.ToString(),
    Capacity = 1,
    Labels = new Dictionary<string, string>
    {
        ["runner.type"] = "docker",
        ["runner.os"] = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "windows" : "linux",
        ["runner.arch"] = RuntimeInformation.OSArchitecture.ToString().ToLowerInvariant()
    }
});

builder.Services.AddHttpClient<ITaskQueue, TaskBrokerClient>(client =>
{
    client.BaseAddress = new Uri(serverUrl);
});
builder.Services.AddHttpClient<RunnerNodeSession>(client =>
{
    client.BaseAddress = new Uri(serverUrl);
});
builder.Services.AddSingleton<IPipelineRunner, PipelineRunner>();
builder.Services.AddSingleton<IStepRunner, StepRunner>();
builder.Services.AddSingleton<IStepExecutor, DockerStepExecutor>();
builder.Services.AddSingleton<IDockerClient>(_ =>
{
    var dockerUri = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
        ? "npipe://./pipe/docker_engine"
        : "unix:///var/run/docker.sock";
    return new DockerClientConfiguration(new Uri(dockerUri)).CreateClient();
});
builder.Services.AddSingleton<IDockerManager, DockerManager>();
builder.Services.AddSingleton<ITaskDispatcher, TaskDispatcher>();
builder.Services.AddHostedService(serviceProvider =>
    serviceProvider.GetRequiredService<RunnerNodeSession>());

var host = builder.Build();

await host.StartAsync();

try
{
    var dispatcher = host.Services.GetRequiredService<ITaskDispatcher>();
    var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();

    await dispatcher.RunAsync(lifetime.ApplicationStopping);
}
catch (OperationCanceledException)
{
}
finally
{
    await host.StopAsync();
}