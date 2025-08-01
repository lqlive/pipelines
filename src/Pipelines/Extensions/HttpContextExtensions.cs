namespace Pipelines.Extensions;

public static class HttpContextExtensions
{
    public static string? GetClientIpAddress(this HttpContext context)
    {
        var headers = new[]
         {
             "CF-Connecting-IP",
             "True-Client-IP",
             "X-Real-IP",
             "X-Forwarded-For",
             "X-Client-IP",
             "X-Forwarded",
             "X-Cluster-Client-IP",
             "Forwarded-For",
             "Forwarded"
        };

        foreach (var header in headers)
        {
            var value = context.Request.Headers[header].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }
        return context.Connection.RemoteIpAddress?.ToString();
    }
}