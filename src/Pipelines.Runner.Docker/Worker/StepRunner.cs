using Docker.DotNet;
using Docker.DotNet.Models;
using Pipelines.Core.Entities.Builds;
using Pipelines.Core.Runner;
using System.Text;
using static Docker.DotNet.MultiplexedStream;

namespace Pipelines.Runner.Docker.Worker;

public class StepRunner
{
    private readonly DockerClient _docker;
    private readonly IJobServer _jobServer;

    public StepRunner(IJobServer jobServer)
    {
        _docker = new DockerClientConfiguration(new Uri(Environment.OSVersion.Platform == PlatformID.Win32NT
            ? "npipe://./pipe/docker_engine"
            : "unix:///var/run/docker.sock")).CreateClient();
        _jobServer = jobServer;
    }

    public async Task RunAsync(Build build, Step step, CancellationToken ct)
    {
        step.Status = StepStatus.Running;
        step.StartedAt = DateTimeOffset.UtcNow;

        var image = string.IsNullOrWhiteSpace(step.Image) ? "ubuntu:22.04" : step.Image!;
        await _docker.Images.CreateImageAsync(new ImagesCreateParameters { FromImage = image }, null, new Progress<JSONMessage>(), ct);

        var cmd = new[] { "/bin/bash", "-lc", step.Script };
        var create = await _docker.Containers.CreateContainerAsync(new CreateContainerParameters
        {
            Image = image,
            Tty = false,
            Cmd = cmd,
            HostConfig = new HostConfig { AutoRemove = true }
        }, ct);

        await _docker.Containers.StartContainerAsync(create.ID, new ContainerStartParameters(), ct);

        var logStream = await _docker.Containers.GetContainerLogsAsync(create.ID, false, new ContainerLogsParameters
        {
            ShowStdout = true,
            ShowStderr = true,
            Follow = true
        }, ct);

        var buffer = new byte[8192];
        ReadResult readResult;
        while ((readResult = await logStream.ReadOutputAsync(buffer, 0, buffer.Length, ct)).Count > 0)
        {
            var text = Encoding.UTF8.GetString(buffer, 0, readResult.Count);
            Console.Write(text);
            _ = _jobServer.AppendLogAsync(build.Id, step.Id, text, ct);
        }

        var waitTask = _docker.Containers.WaitContainerAsync(create.ID, ct);
        var cancelTask = Task.Run(async () =>
        {
            while (!ct.IsCancellationRequested)
            {
                await Task.Delay(1000, ct);
                var isCancelled = await _jobServer.IsCancellationRequestedAsync(build.Id, ct);
                if (isCancelled)
                {
                    try { await _docker.Containers.StopContainerAsync(create.ID, new ContainerStopParameters { WaitBeforeKillSeconds = 5 }, ct); } catch { }
                    break;
                }
            }
        }, ct);

        var completed = await Task.WhenAny(waitTask, cancelTask);
        var wait = completed == waitTask ? await waitTask : new ContainerWaitResponse { StatusCode = -1 };

        step.ExitCode = (int)wait.StatusCode;
        step.FinishedAt = DateTimeOffset.UtcNow;
        step.Status = wait.StatusCode == 0 ? StepStatus.Succeeded : StepStatus.Failed;
    }
}


