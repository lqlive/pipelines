namespace Pipelines.Apis;

public static class WebhooksApi
{
    public static RouteGroupBuilder MapWebhooksApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/webhooks");

        return api;
    }
}
