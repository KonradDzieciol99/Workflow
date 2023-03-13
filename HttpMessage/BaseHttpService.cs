using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using HttpMessage.Extensions;

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

                HttpResponseMessage apiResponse = null;
                switch (apiRequest.ApiType)
                {
                    case ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    case ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }
                apiResponse = await _client.SendAsync(message);


                //if (apiResponse.IsSuccessStatusCode)
                //{
                //    // kod odpowiedzi jest w zakresie 200-299 (sukces), można przetwarzać odpowiedź
                //    string responseBody = await apiResponse.Content.ReadAsStringAsync();
                //    // ...
                //}
                //else
                //{
                //    // kod odpowiedzi nie jest w zakresie sukcesu, należy obsłużyć błąd
                //    string errorResponse = await apiResponse.Content.ReadAsStringAsync();
                //    // ...
                //} //ale jak działają te polityki ?



                var apiContent = await apiResponse.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var apiResponseDto = JsonSerializer.Deserialize<T>(apiContent,options);
                return apiResponseDto;

            }
            catch (Exception e)
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