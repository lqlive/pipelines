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

        environment["HOME"] = scope.HomePath;
        environment["USERPROFILE"] = scope.HomePath;
        environment["PIPELINES_HOME"] = scope.HomePath;
        environment["PIPELINES_WORKSPACE"] = scope.WorkspacePath;
        environment["PIPELINES_WORK_ROOT"] = scope.RootPath;

        return environment;
    }
}
