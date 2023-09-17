using HttpMessage.Models.Exceptions;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HttpMessage;

public class BaseHttpService : IBaseHttpService
{
    private readonly HttpClient _client;

    public BaseHttpService(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }
    public async Task<T> SendAsync<T>(ApiRequest apiRequest)
    {
        try
        {
            _client.DefaultRequestHeaders.Clear();

            HttpRequestMessage message = new();
            message.Headers.Add("Accept", "application/json");
            message.RequestUri = new Uri(apiRequest.Url);
            message.Method = apiRequest.HttpMethod;

            if (apiRequest.Data != null)
                message.Content = new StringContent(JsonSerializer.Serialize(apiRequest.Data), Encoding.UTF8, "application/json");
            
            var apiResponse = await _client.SendAsync(message);

            if (!apiResponse.IsSuccessStatusCode)
                throw apiResponse.StatusCode switch
                {
                    HttpStatusCode.BadRequest => new BadRequestException(await apiResponse.Content.ReadAsStringAsync()),
                    HttpStatusCode.Unauthorized => new UnauthorizedException(await apiResponse.Content.ReadAsStringAsync()),
                    HttpStatusCode.Forbidden => new ForbiddenException(await apiResponse.Content.ReadAsStringAsync()),
                    HttpStatusCode.NotFound => new NotFoundException(await apiResponse.Content.ReadAsStringAsync()),
                    _ => new HttpRequestException(await apiResponse.Content.ReadAsStringAsync()),
                };
            

            var apiContent = await apiResponse.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var apiResponseDto = JsonSerializer.Deserialize<T>(apiContent, options);
            return apiResponseDto;

        }
        catch (Exception)
        {
            throw;
        }
    }
}