namespace Pipelines.Core.Models;

public enum PipelineStatus
{
    Pending,
    Running,
    Succeeded,
    Failed,
    Canceled,
    TimedOut
}