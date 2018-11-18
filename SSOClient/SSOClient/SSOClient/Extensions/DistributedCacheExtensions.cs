using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

public static class DistributedCacheExtensions
{
    public static void Set(this IDistributedCache cahce, string key, object value)
    {
        cahce.SetString(key, JsonConvert.SerializeObject(value));
    }

    public static T Get<T>(this IDistributedCache cahce, string key)
    {
        var value = cahce.GetString(key);
        return value == null ? default(T) :
                              JsonConvert.DeserializeObject<T>(value);
    }
}