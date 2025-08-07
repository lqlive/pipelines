using Microsoft.AspNetCore.Authentication;
using Pipelines.Services.Remotes;

public static class RemoteApi
{
    public static RouteGroupBuilder MapRemoteApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/remotes");

        api.MapGet("/{provider}/authorization/challenge", Challenge);
        api.MapGet("/{provider}/authorization/callback", Callback);

        return api;
    }
    private static async Task<IResult> Challenge(
        HttpContext context,
        string provider,
        RemoteService service,
        CancellationToken cancellationToken)
    {
        // Create AuthenticationProperties with minimal customization
        var properties = new AuthenticationProperties();
        var result = await service.GetChallengeUrlAsync(context, properties, cancellationToken);

        return TypedResults.Redirect(result);
    }

    private static async Task<IResult> Callback(
       HttpContext context,
       RemoteService service,
       string code,
       CancellationToken cancellationToken)
    {
        var properties = new AuthenticationProperties();
        var result = await service.CreateTicketAsync(code, properties, cancellationToken);
        return TypedResults.Json(result);
    }
}