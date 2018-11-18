using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;

namespace SSOClient.Extensions
{
    public static class IConfigurationRootExtensions
    {
        public static T Get<T>(this IConfigurationRoot config, string key)
        {
            var value = config[key];

            if (value == null)
            {
                return default(T);
            }
            else if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(value, typeof(T)); ;
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
        }
    }
}
