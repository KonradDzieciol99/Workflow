using HttpMessage.Models.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HttpMessage;

public abstract class BaseHttpService : IBaseHttpService
{
    private readonly HttpClient _client;

    public BaseHttpService(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<T> SendAsync<T>(ApiRequest apiRequest)
    {
        _client.DefaultRequestHeaders.Clear();

        HttpRequestMessage message = new();
        message.Headers.Add("Accept", "application/json");
        message.RequestUri = new Uri(apiRequest.Url);
        message.Method = apiRequest.HttpMethod;

        if (apiRequest.Data != null)
            message.Content = new StringContent(
                JsonSerializer.Serialize(apiRequest.Data),
                Encoding.UTF8,
                "application/json"
            );

        var apiResponse = await _client.SendAsync(message);

        var apiContent = await apiResponse.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<T>(
            apiContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
    }
}
