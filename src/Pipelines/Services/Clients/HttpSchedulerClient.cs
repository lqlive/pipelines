using Pipelines.Core.Clients;
using Pipelines.Core.Entities.Builds;
using Pipelines.Core.Scheduling;
using System.Net.Http.Json;

namespace Pipelines.Services.Clients;

/// <summary>
/// HTTP implementation of scheduler client
/// </summary>
public class HttpSchedulerClient : ISchedulerClient
{
    private readonly HttpClient _http;

    public HttpSchedulerClient(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("scheduler");
    }

    public async Task<bool> ScheduleJobAsync(Build build, JobPriority priority = JobPriority.Normal, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new { Build = build, Priority = priority };
            var response = await _http.PostAsJsonAsync("/api/scheduler/jobs", request, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> CancelJobAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _http.DeleteAsync($"/api/scheduler/jobs/{jobId}", cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<CancelResponse>(cancellationToken: cancellationToken);
                return result?.Success ?? false;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<QueueStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _http.GetFromJsonAsync<QueueStatistics>("/api/scheduler/stats", cancellationToken);
            return response ?? new QueueStatistics(0, 0, 0, 0, 0, 0, 0, TimeSpan.Zero, TimeSpan.Zero);
        }
        catch
        {
            return new QueueStatistics(0, 0, 0, 0, 0, 0, 0, TimeSpan.Zero, TimeSpan.Zero);
        }
    }

    public async Task<IEnumerable<Build>> GetAssignableJobsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _http.GetFromJsonAsync<Build[]>("/api/scheduler/jobs/assignable", cancellationToken);
            return response ?? Array.Empty<Build>();
        }
        catch
        {
            return Array.Empty<Build>();
        }
    }

    private record CancelResponse(bool Success);
}
