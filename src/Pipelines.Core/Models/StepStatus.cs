namespace Pipelines.Core.Models;

public enum StepStatus
{
    Pending,
    Running,
    Succeeded,
    Failed,
    Skipped,
    Canceled
}