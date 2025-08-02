namespace Pipelines.Apis;
public static class ValidateApi
{ 
    public static RouteGroupBuilder MapValidateApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/validate");
        return api;
    }
}