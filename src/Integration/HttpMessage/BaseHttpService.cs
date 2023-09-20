using HttpMessage.Models.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HttpMessage;

public class BaseHttpService<TDomainEx> : IBaseHttpService<TDomainEx> where TDomainEx : Exception, new()
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
            {
                var responseString = await apiResponse.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ValidationProblemDetails>(responseString, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });


                #pragma warning disable CS8597 // Thrown value may be null.
                throw apiResponse.StatusCode switch
                {
                    HttpStatusCode.BadRequest => (TDomainEx)Activator.CreateInstance(typeof(TDomainEx), result.Errors is null ?( result.Detail,new BadRequestException("") ):( result.Detail, new ValidationException() { Errors=result.Errors})),
                    HttpStatusCode.Unauthorized => (TDomainEx)Activator.CreateInstance(typeof(TDomainEx), result.Detail,new UnauthorizedException("")),
                    HttpStatusCode.Forbidden => (TDomainEx)Activator.CreateInstance(typeof(TDomainEx), result.Detail,new ForbiddenException("")),
                    HttpStatusCode.NotFound => (TDomainEx)Activator.CreateInstance(typeof(TDomainEx), result.Detail,new NotFoundException("")),
                    _ => new HttpRequestException(await apiResponse.Content.ReadAsStringAsync()),
                };
                #pragma warning restore CS8597 // Thrown value may be null.
            }

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