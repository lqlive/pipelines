using Microsoft.Extensions.Primitives;

namespace Pipelines.Extensions;

public static class UserAgentExtensions
{
    public static string ParseDeviceType(this StringValues values)
    {
        var platform = values.FirstOrDefault()?.Trim('"') ?? string.Empty;
        return platform switch
        {
            "Windows" => "Windows",
            "macOS" => "macOS",
            "Linux" => "Linux",
            "Android" => "Android",
            "iOS" => "iOS",
            _ => platform
        };
    }

    public static string ParseDeviceName(this StringValues values)
    {
        var ua = values.FirstOrDefault() ?? string.Empty;

        if (ua.Contains("Microsoft Edge")) return "Edge";
        if (ua.Contains("Chrome") && !ua.Contains("Microsoft Edge")) return "Chrome";
        if (ua.Contains("Firefox")) return "Firefox";
        return "Unknown";
    }
}