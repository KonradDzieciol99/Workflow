﻿using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;

namespace API.Aggregator.Infrastructure;

public class HttpClientTokenForwarderDelegatingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpClientTokenForwarderDelegatingHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor =
            httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        ;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        if (_httpContextAccessor.HttpContext is null)
            throw new InvalidOperationException(
                $"Missing {nameof(_httpContextAccessor.HttpContext)}"
            );

        var authorizationHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];

        if (!string.IsNullOrEmpty(authorizationHeader))
        {
            request.Headers.Add("Authorization", new List<string>() { authorizationHeader });
        }

        var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");

        if (token != null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
