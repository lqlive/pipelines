using System.Net.Http.Json;
using Pipelines.Core.Runner;
using Pipelines.Runner.Docker.Services;

namespace Pipelines.Runner.Docker.Client;

public class HttpJobServerQueue : IJobServerQueue
{
    private readonly HttpClient _http;
    private readonly RunnerRegistrationService _registrationService;

    public HttpJobServerQueue(IHttpClientFactory factory, RunnerRegistrationService registrationService)
    {
        _http = factory.CreateClient("scheduler-server");
        _registrationService = registrationService;
    }

    public async Task EnqueueAsync(object job, CancellationToken cancellationToken = default)
    {
        var request = new { Build = job, Priority = "Normal" };
        var resp = await _http.PostAsJsonAsync("/api/builds/enqueue", request, cancellationToken);
        resp.EnsureSuccessStatusCode();
    }

    public async Task<T?> DequeueAsync<T>(CancellationToken cancellationToken = default) where T : class
    {
        var runnerId = _registrationService.RunnerId;
        if (string.IsNullOrEmpty(runnerId)) return null;

        var resp = await _http.PostAsync($"/api/runners/{runnerId}/jobs/request", null, cancellationToken);
        if (resp.StatusCode == System.Net.HttpStatusCode.NoContent) return null;
        return await resp.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
    }
}


