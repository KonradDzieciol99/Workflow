using System.Threading.Tasks;

namespace HttpMessage
{
    public interface IBaseHttpService
    {
        Task<T> SendAsync<T>(ApiRequest apiRequest);
    }
}