using Pipelines.Core.Models;

namespace Pipelines.Runner.Workstation.Execution;

public sealed class WorkstationExecutionPolicy
{
    public void Validate(StepExecutionContext context)
    {
        var step = context.Step;

        if (step.Privileged)
        {
            throw new InvalidOperationException(
                "Workstation runner does not support privileged execution.");
        }

        if (step.Volumes.Count > 0)
        {
            throw new InvalidOperationException(
                "Workstation runner does not support volume mounts.");
        }

        if (!string.IsNullOrWhiteSpace(step.NetworkMode))
        {
            throw new InvalidOperationException(
                "Workstation runner does not support custom network mode.");
        }

        if (!string.IsNullOrWhiteSpace(step.User))
        {
            throw new InvalidOperationException(
                "Workstation runner does not support per-step user switching.");
        }

        if (step.Detach)
        {
            throw new InvalidOperationException(
                "Workstation runner does not support detached steps.");
        }
    }
}
