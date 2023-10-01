using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;

namespace HttpMessage;

public class ApiRequest
{
    public ApiRequest(HttpMethod httpMethod, string url, object? fromBody = null, IFormFile? fromForm = null)
    {
        HttpMethod = httpMethod ?? throw new ArgumentNullException(nameof(httpMethod));
        Url = url ?? throw new ArgumentNullException(nameof(url));

        if (fromBody is not null && fromForm is not null)
            throw new ArgumentException("you can only send data in one format at a time");

        FromBody = fromBody;
        FromForm = fromForm;
    }

    public HttpMethod HttpMethod { get; }
    public string Url { get; }
    public object? FromBody { get; }
    public IFormFile? FromForm { get; }
}
