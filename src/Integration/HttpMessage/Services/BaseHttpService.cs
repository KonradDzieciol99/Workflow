using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace HttpMessage.Services;

public abstract class BaseHttpService : IBaseHttpService
{
    private readonly HttpClient _client;

    public BaseHttpService(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<T> SendAsync<T>(ApiRequest apiRequest, CancellationToken cancellationToken)
    {
        _client.DefaultRequestHeaders.Clear();

        HttpRequestMessage message = new();
        message.Headers.Add("Accept", "application/json");
        message.RequestUri = new Uri(apiRequest.Url);
        message.Method = apiRequest.HttpMethod;

        if (apiRequest.FromBody is not null)
            message.Content = new StringContent(
                JsonSerializer.Serialize(apiRequest.FromBody),
                Encoding.UTF8,
                "application/json"
            );

        StreamContent? streamContent = null;
        Stream? fileStream = null;
        MultipartFormDataContent? content = null;

        if (apiRequest.FromForm is not null)
        {
            content = new MultipartFormDataContent();
            fileStream = apiRequest.FromForm.OpenReadStream();
            streamContent = new StreamContent(fileStream);
            content.Add(streamContent, "file", apiRequest.FromForm.FileName);
            message.Content = content;
        }

        var apiResponse = await _client.SendAsync(message, cancellationToken);

        streamContent?.Dispose();
        fileStream?.Dispose();
        content?.Dispose();

        var apiContent = await apiResponse.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<T>(
            apiContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
    }
}
