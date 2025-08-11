namespace Pipelines.Services.Identity;

public class IdentityService(IHttpContextAccessor context) : IIdentityService
{
    public string GetUserIdentity()
        => context.HttpContext?.User.FindFirst("sub")?.Value ?? throw new ArgumentNullException();

    public string GetUserName()
        => context.HttpContext?.User.Identity?.Name ?? throw new ArgumentNullException();
}