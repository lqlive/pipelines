using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pipelines.Core.Abstractions;
using Pipelines.Core.Models;
using Pipelines.Runner.Listener;
using Pipelines.Runner.Workstation;
using Pipelines.Runner.Worker;


var builder = Host.CreateApplicationBuilder(args);
var runnerId = Guid.NewGuid();
var serverUrl = "http://localhost:5011";
var runnerOs = "windows";
var runnerArch = "amd64";

builder.Services.AddSingleton(new RunnerProfile
{
    RunnerId = runnerId,
    Type = "workstation",
    Os = runnerOs,
    Architecture = runnerArch,
    Capacity = 1,
    Labels = new Dictionary<string, string>
    {
        ["runner.type"] = "workstation",
        ["runner.os"] = runnerOs,
        ["runner.arch"] = runnerArch
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
builder.Services.AddSingleton<IStepExecutor, WorkstationStepExecutor>();
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