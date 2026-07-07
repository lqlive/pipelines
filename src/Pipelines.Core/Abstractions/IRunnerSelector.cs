using Pipelines.Core.Configuration;
using Pipelines.Core.Models;

namespace Pipelines.Abstractions;

public interface IRunnerSelector
{
    bool Matches(PipelineConfiguration pipeline, RunnerProfile runner);
}
