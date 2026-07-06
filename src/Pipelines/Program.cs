using System.Text.Json.Serialization;
using Pipelines;
using Pipelines.Abstractions;
using Pipelines.Apis;
using Pipelines.Core.Stores;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ITaskStore, InMemoryTaskStore>();
builder.Services.AddSingleton<ITaskBroker, TaskBroker>();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
var app = builder.Build();

app.MapTasks();
app.MapWebhooks();

await app.RunAsync();