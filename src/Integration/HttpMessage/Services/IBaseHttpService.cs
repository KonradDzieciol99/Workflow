using System;
using System.Threading.Tasks;

namespace HttpMessage.Services;

public interface IBaseHttpService
{
    Task<T> SendAsync<T>(ApiRequest apiRequest);
}
