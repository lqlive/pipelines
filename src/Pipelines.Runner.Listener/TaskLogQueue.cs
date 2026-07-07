using System.Net.Http.Json;
using System.Threading.Channels;
using Microsoft.Extensions.Hosting;
using Pipelines.Core.Abstractions;
using Pipelines.Core.Models;

namespace Pipelines.Runner.Listener;

public sealed class TaskLogQueue : ITaskLogQueue, IHostedService
{
    private const int MaxBatchSize = 100;

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly Channel<TaskLogEntry> _channel;
    private readonly CancellationTokenSource _stoppingCts = new();

    private Task? _processingTask;

    public TaskLogQueue(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _channel = Channel.CreateUnbounded<TaskLogEntry>(
            new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false
            });
    }

    public async Task QueueAsync(
        TaskLogEntry entry,
        CancellationToken cancellationToken = default)
    {
        await _channel.Writer.WriteAsync(entry, cancellationToken);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _processingTask = ProcessAsync(_stoppingCts.Token);
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _channel.Writer.TryComplete();
        await _stoppingCts.CancelAsync();

        if (_processingTask is null)
        {
            return;
        }

        await Task.WhenAny(
            _processingTask,
            Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken));
    }

    private async Task ProcessAsync(CancellationToken cancellationToken)
    {
        var buffer = new List<TaskLogEntry>(MaxBatchSize);

        try
        {
            await foreach (var entry in _channel.Reader.ReadAllAsync(cancellationToken))
            {
                buffer.Add(entry);

                while (buffer.Count < MaxBatchSize &&
                       _channel.Reader.TryRead(out var next))
                {
                    buffer.Add(next);
                }

                await DispatchBatchAsync(buffer, cancellationToken);
                buffer.Clear();
            }
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            await FlushRemainingAsync(buffer, CancellationToken.None);
        }
    }

    private async Task FlushRemainingAsync(
        List<TaskLogEntry> buffer,
        CancellationToken cancellationToken)
    {
        while (_channel.Reader.TryRead(out var entry))
        {
            buffer.Add(entry);

            if (buffer.Count >= MaxBatchSize)
            {
                await DispatchBatchAsync(buffer, cancellationToken);
                buffer.Clear();
            }
        }

        if (buffer.Count > 0)
        {
            await DispatchBatchAsync(buffer, cancellationToken);
            buffer.Clear();
        }
    }

    private async Task DispatchBatchAsync(
        IReadOnlyList<TaskLogEntry> entries,
        CancellationToken cancellationToken)
    {
        if (entries.Count == 0)
        {
            return;
        }

        var client = _httpClientFactory.CreateClient("pipelines");

        foreach (var group in entries.GroupBy(entry => entry.TaskId))
        {
            using var response = await client.PostAsJsonAsync(
                $"/tasks/{group.Key}/logs/batch",
                new AppendTaskLogsRequest(group.ToList()),
                cancellationToken);

            response.EnsureSuccessStatusCode();
        }
    }

    private sealed record AppendTaskLogsRequest(
        IReadOnlyList<TaskLogEntry> Entries);
}