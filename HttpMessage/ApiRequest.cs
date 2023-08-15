using System.Net.Http;

namespace HttpMessage
{
    public class ApiRequest
    {
        public HttpMethod HttpMethod { get; set; }
        public string Url { get; set; }
        public object? Data { get; set; }
        public string AccessToken { get; set; }
    }
}