namespace Pipelines.Apis;

public static class UploadApi
{
    
    public static RouteGroupBuilder MapUploadApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/upload");

        api.MapPut("/avatars", PutAvatars);
        return api;
    }
    private Task PutAvatars()
    {
        throw new NotImplementedException();
    }
}
