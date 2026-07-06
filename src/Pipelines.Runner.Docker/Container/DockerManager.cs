using System.Text;
using Docker.DotNet;
using Docker.DotNet.Models;
using Pipelines.Core.Configuration;
using System.Runtime.CompilerServices;

namespace Pipelines.Runner.Docker.Container;

public class DockerManager : IDockerManager
{
    private readonly IDockerClient _client;
    public DockerManager(IDockerClient client)
    {
        _client = client;
    }

    public async Task<string> CreateContainerAsync(DockerContainerSpec spec, CancellationToken cancellationToken = default)
    {
        var response = await _client.Containers.CreateContainerAsync(
        new CreateContainerParameters
        {
            Name = spec.Name,
            Image = spec.Image,
            User = spec.User,
            WorkingDir = spec.WorkingDirectory,
            Entrypoint = spec.Entrypoint.ToList(),
            Cmd = spec.Command.ToList(),
            Env = spec.Environment
                .Select(x => $"{x.Key}={x.Value}")
                .ToList(),
            Labels = spec.Labels.ToDictionary(x => x.Key, x => x.Value),
            AttachStdout = true,
            AttachStderr = true,
            Tty = false,
            HostConfig = new HostConfig
            {
                NetworkMode = spec.NetworkMode,
                Privileged = spec.Privileged,
                Mounts = spec.Volumes.Select(ToDockerMount).ToList()
            }
        },
        cancellationToken);
        return response.ID;
    }

    public async Task<string> CreateNetworkAsync(string name, CancellationToken cancellationToken = default)
    {
        var response = await _client.Networks.CreateNetworkAsync(
          new NetworksCreateParameters
          {
              Name = name,
              Driver = "bridge",
              CheckDuplicate = true,
              Labels = new Dictionary<string, string>
              {
                  ["pipelines.managed"] = "true"
              }
          },
          cancellationToken);
        return response.ID;
    }

    public async Task PullImageAsync(string image, PullPolicy pullPolicy, CancellationToken cancellationToken = default)
    {
        if (pullPolicy == PullPolicy.Never)
        {
            return;
        }
        if (pullPolicy == PullPolicy.IfNotExists && await ImageExistsAsync(image, cancellationToken))
        {
            return;
        }

        var (fromImage, tag) = SplitImageName(image);

        await _client.Images.CreateImageAsync(
            new ImagesCreateParameters
            {
                FromImage = fromImage,
                Tag = tag
            },
            authConfig: null,
            progress: new Progress<JSONMessage>(),
            cancellationToken);
    }

    public Task RemoveContainerAsync(string containerId, bool force = true, CancellationToken cancellationToken = default)
    {
        return _client.Containers.RemoveContainerAsync(
          containerId,
          new ContainerRemoveParameters
          {
              Force = force,
              RemoveVolumes = true
          },
          cancellationToken);
    }

    public Task RemoveNetworkAsync(string name, CancellationToken cancellationToken = default)
    {
        return _client.Networks.DeleteNetworkAsync(name, cancellationToken);
    }

    public Task StartContainerAsync(string containerId, CancellationToken cancellationToken = default)
    {
        return _client.Containers.StartContainerAsync(containerId, new ContainerStartParameters(), cancellationToken);
    }

    public Task StopContainerAsync(string containerId, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        return _client.Containers.StopContainerAsync(
              containerId,
              new ContainerStopParameters
              {
                  WaitBeforeKillSeconds = (uint)Math.Max(0, timeout.TotalSeconds)
              },
              cancellationToken);
    }

    public async IAsyncEnumerable<DockerLogEntry> StreamLogsAsync(string containerId, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var stream = await _client.Containers.GetContainerLogsAsync(
            containerId,
            tty: false,
            new ContainerLogsParameters
            {
                ShowStdout = true,
                ShowStderr = true,
                Follow = true,
                Timestamps = false
            },
            cancellationToken);

        var buffer = new byte[81920];
        while (!cancellationToken.IsCancellationRequested)
        {
            var result = await stream.ReadOutputAsync(
                buffer,
                0,
                buffer.Length,
                cancellationToken);

            if (result.EOF)
            {
                yield break;
            }

            var text = Encoding.UTF8.GetString(buffer, 0, result.Count);
            var source = result.Target == MultiplexedStream.TargetStream.StandardError
                ? "stderr"
                : "stdout";

            yield return new DockerLogEntry
            {
                Stream = source,
                Text = text,
                Timestamp = DateTimeOffset.UtcNow
            };
        }
    }

    public async Task<int> WaitContainerAsync(string containerId, CancellationToken cancellationToken = default)
    {
        var response = await _client.Containers.WaitContainerAsync(containerId, cancellationToken);
        return (int)response.StatusCode;
    }

    private async Task<bool> ImageExistsAsync(string image, CancellationToken cancellationToken)
    {
        try
        {
            await _client.Images.InspectImageAsync(image, cancellationToken);
            return true;
        }
        catch (DockerImageNotFoundException)
        {
            return false;
        }
    }

    private static (string FromImage, string Tag) SplitImageName(string image)
    {
        var slashIndex = image.LastIndexOf('/');
        var colonIndex = image.LastIndexOf(':');
        if (colonIndex > slashIndex)
        {
            return (image[..colonIndex], image[(colonIndex + 1)..]);
        }
        return (image, "latest");
    }

    private static Mount ToDockerMount(DockerVolumeMount volume)
    {
        return new Mount
        {
            Type = volume.Type switch
            {
                DockerVolumeMountType.Volume => "volume",
                DockerVolumeMountType.Tmpfs => "tmpfs",
                _ => "bind"
            },
            Source = volume.Source,
            Target = volume.Target,
            ReadOnly = volume.ReadOnly
        };
    }
}