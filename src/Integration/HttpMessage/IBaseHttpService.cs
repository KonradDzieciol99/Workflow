using System;
using System.Threading.Tasks;

namespace HttpMessage;

public interface IBaseHttpService<TDomainEx> where TDomainEx : Exception, new()
{
    Task<T> SendAsync<T>(ApiRequest apiRequest);
}