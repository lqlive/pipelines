namespace Pipelines.Runner.Listener;

public sealed class TaskDispatcherOptions
{
    public TimeSpan IdleDelay { get; init; } = TimeSpan.FromSeconds(2);

    public TimeSpan LeaseRenewalInterval { get; init; } = TimeSpan.FromSeconds(30);

    public TimeSpan LeaseDuration { get; init; } = TimeSpan.FromMinutes(5);
}