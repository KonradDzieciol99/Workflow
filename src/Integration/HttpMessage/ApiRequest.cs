using System;
using System.Net.Http;

namespace HttpMessage;

public class ApiRequest
{
    public ApiRequest(HttpMethod httpMethod, string url, object? data)
    {
        HttpMethod = httpMethod ?? throw new ArgumentNullException(nameof(httpMethod));
        Url = url ?? throw new ArgumentNullException(nameof(url));
        Data = data;
    }

    public HttpMethod HttpMethod { get; }
    public string Url { get; }
    public object? Data { get; }
}
