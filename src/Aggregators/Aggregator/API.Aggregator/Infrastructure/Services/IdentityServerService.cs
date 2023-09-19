﻿using API.Aggregator.Application.Commons.Models;
using HttpMessage;
using System.Text;

namespace API.Aggregator.Infrastructure.Services;

public class IdentityServerService : BaseHttpService, IIdentityServerService
{
    private readonly string _identityUrl;

    public IdentityServerService(HttpClient client, IConfiguration configuration) : base(client)
    {
        _identityUrl = configuration.GetValue<string>("urls:internal:IdentityHttp") ?? throw new ArgumentNullException(_identityUrl);
    }

    public async Task<UserDto?> CheckIfUserExistsAsync(string email)
    {
        var sb = new StringBuilder(_identityUrl);
        sb.Append($"/api/IdentityUser/CheckIfUserExists/{email}");

        return await SendAsync<UserDto?>(new ApiRequest(HttpMethod.Get, sb.ToString(), null));
    }
    public async Task<List<UserDto>> SearchAsync(string email, int take, int skip)
    {
        var sb = new StringBuilder(_identityUrl);
        sb.Append($"/api/IdentityUser/search/{email}?take={take}&skip={skip}");

        return await SendAsync<List<UserDto>>(new ApiRequest(HttpMethod.Get, sb.ToString(), null));
    }
}