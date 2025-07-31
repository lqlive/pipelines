namespace Pipelines.Core.Entities.Users;
public enum EmailNotification
{
    None = 0,
    FailuresOnly = 1,
    SuccessOnly = 2,
    AllBuilds = 3
}