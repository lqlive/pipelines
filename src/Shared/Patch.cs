using System.Reflection;
using System.Text.Json;

namespace Microsoft.AspNetCore.Http;

public sealed class Patch<TModel> where TModel : class
{
    private readonly IDictionary<PropertyInfo, object> _changedProperties = new Dictionary<PropertyInfo, object>();

    public void ApplyTo<TTarget>(TTarget target) where TTarget : class
    {
        if (target is null)
        {
            throw new ArgumentNullException(nameof(target));
        }

        var targetType = typeof(TTarget);

        foreach (var property in _changedProperties)
        {
            var targetProperty = targetType.GetProperty(property.Key.Name,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (targetProperty != null && targetProperty.CanWrite)
            {

                object? value = ChangeType(property.Value, targetProperty.PropertyType);
                targetProperty.SetValue(target, value);
            }
        }
    }

    private static object? ChangeType(object value, Type type)
    {
        try
        {
            if (type == typeof(Guid))
            {
                return Guid.Parse((string)value);
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (value is null)
                {
                    return null;
                }

                type = Nullable.GetUnderlyingType(type)!;
            }

            return Convert.ChangeType(value, type!);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Custom parameter binding method for minimal APIs
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <param name="parameter">Parameter information</param>
    /// <returns>A task that represents the asynchronous binding operation</returns>
    public static async ValueTask<Patch<TModel>> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        using var reader = new StreamReader(context.Request.Body);
        var json = await reader.ReadToEndAsync();

        if (string.IsNullOrWhiteSpace(json))
            return new Patch<TModel>();

        var patch = new Patch<TModel>();

        using var doc = JsonDocument.Parse(json);

        var properties = doc.RootElement
            .EnumerateObject()
            .Select(prop => new
            {
                PropertyInfo = typeof(TModel).GetProperty(prop.Name,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance),
                Value = GetJsonValue(prop.Value)
            })
            .Where(x => x.PropertyInfo is not null && x.Value is not null);

        foreach (var prop in properties)
        {
            patch._changedProperties.Add(prop.PropertyInfo!, prop.Value!);
        }

        return patch;
    }

    /// <summary>
    /// Extracts the appropriate .NET value from a JsonElement
    /// </summary>
    private static object? GetJsonValue(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => element.TryGetInt64(out var longValue) ? longValue : element.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            JsonValueKind.Undefined => null,
            _ => element.GetRawText()
        };
    }
}