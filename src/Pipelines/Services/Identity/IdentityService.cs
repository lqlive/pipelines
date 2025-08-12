namespace Pipelines.Services.Identity;

public class IdentityService(IHttpContextAccessor context)
{
    public string GetUserIdentity()
        => context.HttpContext?.User.FindFirst("sub")?.Value ?? string.Empty;

    public string GetUserName()
        => context.HttpContext?.User.Identity?.Name ?? string.Empty;
}