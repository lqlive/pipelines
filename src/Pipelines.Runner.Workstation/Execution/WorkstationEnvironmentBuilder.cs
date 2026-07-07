using Pipelines.Core.Models;

namespace Pipelines.Runner.Workstation.Execution;

public sealed class WorkstationEnvironmentBuilder
{
    private readonly WorkstationExecutionOptions _options;

    public WorkstationEnvironmentBuilder(WorkstationExecutionOptions options)
    {
        _options = options;
    }

    public IReadOnlyDictionary<string, string> Build(
        WorkstationExecutionScope scope,
        StepExecutionContext context)
    {
        var environment = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var name in _options.AllowedInheritedEnvironmentVariables)
        {
            var value = Environment.GetEnvironmentVariable(name);
            if (!string.IsNullOrEmpty(value))
            {
                environment[name] = value;
            }
        }

        foreach (var item in context.Environment)
        {
            environment[item.Key] = item.Value;
        }

        foreach (var item in context.Step.Environment)
        {
            environment[item.Key] = item.Value;
        }

        var appDataPath = Path.Combine(scope.HomePath, "AppData", "Roaming");
        var localAppDataPath = Path.Combine(scope.HomePath, "AppData", "Local");
        var nugetPackagesPath = Path.Combine(scope.HomePath, ".nuget", "packages");
        var nugetHttpCachePath = Path.Combine(localAppDataPath, "NuGet", "v3-cache");
        var nugetPluginsCachePath = Path.Combine(localAppDataPath, "NuGet", "plugins-cache");
        var tempPath = Path.Combine(scope.RootPath, "tmp");

        Directory.CreateDirectory(appDataPath);
        Directory.CreateDirectory(localAppDataPath);
        Directory.CreateDirectory(nugetPackagesPath);
        Directory.CreateDirectory(nugetHttpCachePath);
        Directory.CreateDirectory(nugetPluginsCachePath);
        Directory.CreateDirectory(tempPath);

        environment["HOME"] = scope.HomePath;
        environment["USERPROFILE"] = scope.HomePath;
        environment["HOMEDRIVE"] = Path.GetPathRoot(scope.HomePath)?.TrimEnd(Path.DirectorySeparatorChar) ?? string.Empty;
        environment["HOMEPATH"] = Path.DirectorySeparatorChar + Path.GetRelativePath(
            Path.GetPathRoot(scope.HomePath) ?? scope.HomePath,
            scope.HomePath);
        environment["APPDATA"] = appDataPath;
        environment["LOCALAPPDATA"] = localAppDataPath;
        environment["TEMP"] = tempPath;
        environment["TMP"] = tempPath;
        environment["DOTNET_CLI_HOME"] = scope.HomePath;
        environment["NUGET_PACKAGES"] = nugetPackagesPath;
        environment["NUGET_HTTP_CACHE_PATH"] = nugetHttpCachePath;
        environment["NUGET_PLUGINS_CACHE_PATH"] = nugetPluginsCachePath;
        environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
        environment["DOTNET_SKIP_FIRST_TIME_EXPERIENCE"] = "1";
        environment["PIPELINES_HOME"] = scope.HomePath;
        environment["PIPELINES_WORKSPACE"] = scope.WorkspacePath;
        environment["PIPELINES_WORK_ROOT"] = scope.RootPath;

        return environment;
    }
}
