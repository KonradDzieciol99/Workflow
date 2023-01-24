using Microsoft.Extensions.Caching.Distributed;
//using StackExchange.Redis;
using System.Text.Json;

namespace WorkflowApi.Extensions
{
    public static class DatabaseAsyncExtensions
    {
    //    public static async Task SetRecordAsync<T>(this IDatabaseAsync cache,
    //string recordId,
    //T data,
    //TimeSpan? absoluteExpireTime = null)
    //    {

    //        var AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromSeconds(120);

    //        var jsonData = JsonSerializer.Serialize(data);
    //        await cache.StringSetAsync(recordId, jsonData, AbsoluteExpirationRelativeToNow);
    //    }

    //    public static async Task<T> GetRecordAsync<T>(this IDatabaseAsync cache, string recordId)
    //    {
    //        var jsonData = await cache.StringGetAsync(recordId);

    //        if (jsonData.IsNullOrEmpty)
    //        {
    //            return default(T);
    //        }

    //        return JsonSerializer.Deserialize<T>(jsonData);
    //    }

        //public static async Task<T> GetRecordAsync<T>(this IDatabaseAsync cache, string[] recordIds)
        //{
        //    RedisKey[] redisKeys = new RedisKey[recordIds.Length];
        //    foreach (string item in recordIds)
        //    {
        //        redisKeys.Append(item);
        //    }
        //    var jsonData = await cache.StringGetAsync(redisKeys);

        //    if (jsonData is null || jsonData.Length == 0)
        //    {
        //        return default(T);
        //    }

        //    return JsonSerializer.Deserialize<array[T]>(jsonData);
        //}
    }
}
