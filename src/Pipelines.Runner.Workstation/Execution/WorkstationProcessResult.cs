using Pipelines.Core.Configuration;
using Pipelines.Core.Models;

namespace Pipelines.Runner.Workstation.Execution;

public sealed class WorkstationProcessResult
{
    public int ExitCode { get; init; }
    public StepStatus Status { get; init; }
    public TimeSpan Duration { get; init; }

    public StepResult ToStepResult(StepConfiguration step)
    {
        var status = Status == StepStatus.Failed && step.Failure == FailurePolicy.Ignore
            ? StepStatus.Succeeded
            : Status;

        return new StepResult
        {
            StepName = step.Name,
            ExitCode = ExitCode,
            Status = status,
            Duration = Duration
        };
    }
}
