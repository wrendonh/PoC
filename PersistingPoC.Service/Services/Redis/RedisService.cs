using EasyCaching.Core;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json;
using PersistingPoC.Service.Interfaces;
using StackExchange.Redis;
using System;
using System.Text;
using System.Threading.Tasks;

namespace PersistingPoC.Service.Services.Redis
{
    public class RedisService : IRedisService
    {
        private readonly IEasyCachingProvider _cachingProvider;
        private IEasyCachingProviderFactory _cachingProviderFactory;

        public RedisService(IEasyCachingProviderFactory cachingProviderFactory)
        {
            _cachingProviderFactory = cachingProviderFactory;
            _cachingProvider = _cachingProviderFactory.GetCachingProvider("redis1");
        }

        public async Task Set<T>(string key, T value)
        {
            var json = JsonConvert.SerializeObject(value);
            byte[] bytes = Encoding.ASCII.GetBytes(json);
            await _cachingProvider.SetAsync(key, bytes, TimeSpan.FromHours(2));
        }

        public async Task<T> Get<T>(string key)
        {
            var value = await _cachingProvider.GetAsync<byte[]>(key);            
            if (value.HasValue)
            {
                string jsonStr = Encoding.UTF8.GetString(value.Value);
                var obj = JsonConvert.DeserializeObject<T>(jsonStr);
                return obj;
            }
            return default;
        }
    }
}
