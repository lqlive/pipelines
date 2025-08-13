using System.Text;

namespace Pipelines.Services.Builds;

public class LogStorageService
{
    private readonly string _root;

    public LogStorageService()
    {
        _root = Environment.GetEnvironmentVariable("PIPELINES_LOG_DIR") ?? Path.GetFullPath("data/logs");
        Directory.CreateDirectory(_root);
    }

    private string GetPath(Guid buildId, Guid? stepId)
    {
        var dir = Path.Combine(_root, buildId.ToString("N"));
        Directory.CreateDirectory(dir);
        var file = stepId.HasValue ? Path.Combine(dir, $"{stepId: N}.log").Replace(" ", "") : Path.Combine(dir, "build.log");
        return file;
    }

    public async Task AppendAsync(Guid buildId, Guid? stepId, string content, CancellationToken ct)
    {
        var path = GetPath(buildId, stepId);
        await File.AppendAllTextAsync(path, content, Encoding.UTF8, ct);
    }
}


