using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TestsHelpers.Extensions;

public static class ObjectExtensions
{
    public static string ToQueryString(this object obj)
    {
        string json = JsonSerializer.Serialize(obj);
        var dictionary =
            JsonSerializer.Deserialize<Dictionary<string, object>>(json)
            ?? throw new InvalidOperationException(
                $"Deserialization to dictionary failed for JSON: {json}"
            );

        var properties = dictionary
            .Where(pair => pair.Value != null)
            .Select(pair => $"{pair.Key}={WebUtility.UrlEncode(pair.Value.ToString())}");

        return string.Join("&", properties.ToArray());
    }
}
