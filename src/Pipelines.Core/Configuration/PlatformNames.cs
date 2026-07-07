namespace Pipelines.Core.Configuration;

public static class PlatformNames
{
    private static readonly IReadOnlyDictionary<string, string> OsAliases =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["win"] = "windows",
            ["win32"] = "windows",
            ["windows_nt"] = "windows",
            ["mac"] = "darwin",
            ["macos"] = "darwin",
            ["osx"] = "darwin"
        };

    private static readonly IReadOnlyDictionary<string, string> ArchitectureAliases =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["x64"] = "amd64",
            ["x86_64"] = "amd64",
            ["aarch64"] = "arm64"
        };

    public static string Os(string value)
    {
        return Resolve(value, OsAliases);
    }

    public static string Architecture(string value)
    {
        return Resolve(value, ArchitectureAliases);
    }

    private static string Resolve(
        string value,
        IReadOnlyDictionary<string, string> aliases)
    {
        var name = value.Trim().ToLowerInvariant();

        return aliases.TryGetValue(name, out var canonical)
            ? canonical
            : name;
    }
}
