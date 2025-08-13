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

    public async Task<(byte[] data, long nextOffset)> ReadAsync(Guid buildId, Guid? stepId, long offset, int bytes, CancellationToken ct)
    {
        var path = GetPath(buildId, stepId);
        if (!File.Exists(path)) return (Array.Empty<byte>(), offset);
        await using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        if (offset > fs.Length) offset = fs.Length;
        fs.Seek(offset, SeekOrigin.Begin);
        var buffer = new byte[Math.Min(bytes, (int)Math.Max(0, fs.Length - offset))];
        var read = await fs.ReadAsync(buffer, 0, buffer.Length, ct);
        if (read < buffer.Length)
        {
            Array.Resize(ref buffer, read);
        }
        return (buffer, offset + read);
    }
}


