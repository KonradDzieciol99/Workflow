using HttpMessage.Models.Exceptions;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HttpMessage
{
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
                //var client = httpClient.CreateClient("MangoAPI");
                HttpRequestMessage message = new HttpRequestMessage();
                message.Headers.Add("Accept", "application/json");
                message.RequestUri = new Uri(apiRequest.Url);
                message.Method = apiRequest.HttpMethod;
                message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiRequest.AccessToken);
                _client.DefaultRequestHeaders.Clear();
                if (apiRequest.Data != null)
                {
                    message.Content = new StringContent(JsonSerializer.Serialize(apiRequest.Data),
                        Encoding.UTF8, "application/json");
                }

                if (!string.IsNullOrEmpty(apiRequest.AccessToken))
                {
                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiRequest.AccessToken);
                }

                HttpResponseMessage apiResponse = await _client.SendAsync(message);


                if (!apiResponse.IsSuccessStatusCode)
                {
                    switch (apiResponse.StatusCode)
                    {
                        case HttpStatusCode.BadRequest:
                            throw new BadRequestException(await apiResponse.Content.ReadAsStringAsync());
                        case HttpStatusCode.Unauthorized:
                            throw new UnauthorizedException(await apiResponse.Content.ReadAsStringAsync());
                        case HttpStatusCode.Forbidden:
                            throw new ForbiddenException(await apiResponse.Content.ReadAsStringAsync());
                        case HttpStatusCode.NotFound:
                            throw new NotFoundException(await apiResponse.Content.ReadAsStringAsync());
                        default:
                            throw new HttpRequestException(await apiResponse.Content.ReadAsStringAsync());
                    }
                }

                //ale jak działają te polityki longpoly?

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
                //var dto = new ResponseDto
                //{
                //    DisplayMessage = "Error",
                //    ErrorMessages = new List<string> { Convert.ToString(e.Message) },
                //    IsSuccess = false
                //};
                //var res = JsonSerializer.Serialize(dto);
                //var apiResponseDto = JsonSerializer.Deserialize<T>(res);
                //return apiResponseDto;
            }
        }
    }
}