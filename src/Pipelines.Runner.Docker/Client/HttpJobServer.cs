using Pipelines.Core.Runner;
using System.Net.Http.Json;

namespace Pipelines.Runner.Docker.Client;

public class HttpJobServer : IJobServer
{
    private readonly HttpClient _http;

    public HttpJobServer(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("pipelines-server"); // Still connects to Pipelines API for logs
    }

    public async Task AppendLogAsync(Guid buildId, Guid? stepId, string content, CancellationToken cancellationToken = default)
    {
        using var msg = new HttpRequestMessage(HttpMethod.Post, $"/api/builds/{buildId}/logs?stepId={stepId}")
        {
            Content = new StringContent(content, System.Text.Encoding.UTF8, "text/plain")
        };
        await _http.SendAsync(msg, cancellationToken);
    }

    public async Task<bool> IsCancellationRequestedAsync(Guid buildId, CancellationToken cancellationToken = default)
    {
        var resp = await _http.GetFromJsonAsync<CancelResponse>($"/api/builds/{buildId}/canceled", cancellationToken);
        return resp?.Canceled ?? false;
    }
}

public record CancelResponse(bool Canceled);


