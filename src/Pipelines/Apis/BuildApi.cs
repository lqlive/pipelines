public static class BuildApi
{
    public static RouteGroupBuilder MapBuildApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/builds");

        return api;
    }
}
