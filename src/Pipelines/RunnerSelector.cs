using Pipelines.Abstractions;
using Pipelines.Core.Configuration;
using Pipelines.Core.Models;

namespace Pipelines;

public sealed class RunnerSelector : IRunnerSelector
{
    public bool Matches(PipelineConfiguration pipeline, RunnerProfile runner)
    {
        return MatchesType(pipeline, runner) &&
            MatchesPlatform(pipeline, runner) &&
            MatchesNodeSelector(pipeline.Node, runner.Labels);
    }

    private static bool MatchesType(PipelineConfiguration pipeline, RunnerProfile runner)
    {
        return string.Equals(
            pipeline.Type,
            runner.Type,
            StringComparison.OrdinalIgnoreCase);
    }

    private static bool MatchesPlatform(PipelineConfiguration pipeline, RunnerProfile runner)
    {
        var runnerOs = GetLabelOrDefault(runner, "runner.os", runner.Os);
        var runnerArch = GetLabelOrDefault(runner, "runner.arch", runner.Architecture);

        return string.Equals(
                PlatformNames.Os(pipeline.Platform.Os),
                PlatformNames.Os(runnerOs),
                StringComparison.OrdinalIgnoreCase) &&
            string.Equals(
                PlatformNames.Architecture(pipeline.Platform.Arch),
                PlatformNames.Architecture(runnerArch),
                StringComparison.OrdinalIgnoreCase);
    }

    private static bool MatchesNodeSelector(
        IReadOnlyDictionary<string, string> selector,
        IReadOnlyDictionary<string, string> labels)
    {
        foreach (var item in selector)
        {
            if (!labels.TryGetValue(item.Key, out var value))
            {
                return false;
            }

            if (!string.Equals(value, item.Value, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return true;
    }

    private static string GetLabelOrDefault(
        RunnerProfile runner,
        string key,
        string fallback)
    {
        return runner.Labels.TryGetValue(key, out var value)
            ? value
            : fallback;
    }
}
