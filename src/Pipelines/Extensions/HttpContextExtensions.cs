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

    public static string GetDeviceType(this HttpContext context)
    {
        return context.Request.Headers["sec-ch-ua-platform"].ParseDeviceType();
    }
    public static string GetDeviceName(this HttpContext context)
    {
        return context.Request.Headers["sec-ch-ua"].ParseDeviceName();
    }
}