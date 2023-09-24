using System.Net.Http.Headers;

namespace TestsHelpers.Extensions;
public static class HttpClientExtensions
{
    public static void SetHeaders(this HttpClient client, string userId, string userEmail)
    {
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthHandler.AuthenticationScheme);
        client.DefaultRequestHeaders.Add(TestAuthHandler.UserId, userId);
        client.DefaultRequestHeaders.Add(TestAuthHandler.UserEmail, userEmail);
    }
}
