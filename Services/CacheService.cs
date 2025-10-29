using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;
using System.Text.Json;

namespace VehicleManagementAPI.Services
{
    public class CacheService : ICacheService
    {
        private readonly IConnectionMultiplexer? _redis;
        private readonly IMemoryCache _memoryCache;
        private readonly bool _useRedis;
        private readonly MemoryCacheEntryOptions _defaultMemoryOptions;

        public CacheService(IConnectionMultiplexer? redis, IMemoryCache memoryCache)
        {
            _redis = redis;
            _memoryCache = memoryCache;
            _useRedis = _redis?.IsConnected == true;
            _defaultMemoryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(10));
            
            Console.WriteLine(_useRedis ? "✅ Usando Redis para cache" : "🔶 Usando MemoryCache como fallback");
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            if (_useRedis && _redis != null)
            {
                try
                {
                    var db = _redis.GetDatabase();
                    var cachedValue = await db.StringGetAsync(key);
                    return cachedValue.HasValue ? 
                        JsonSerializer.Deserialize<T>(cachedValue!) : default(T);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro no Redis, usando MemoryCache: {ex.Message}");
                }
            }

            return _memoryCache.TryGetValue(key, out T? value) ? value : default(T);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            if (_useRedis && _redis != null)
            {
                try
                {
                    var db = _redis.GetDatabase();
                    var serializedValue = JsonSerializer.Serialize(value);
                    await db.StringSetAsync(key, serializedValue, expiry);
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro no Redis, usando MemoryCache: {ex.Message}");
                }
            }

            var options = expiry.HasValue ? 
                new MemoryCacheEntryOptions().SetAbsoluteExpiration(expiry.Value) : 
                _defaultMemoryOptions;
            
            _memoryCache.Set(key, value, options);
        }

        public async Task RemoveAsync(string key)
        {
            if (_useRedis && _redis != null)
            {
                try
                {
                    var db = _redis.GetDatabase();
                    await db.KeyDeleteAsync(key);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro no Redis: {ex.Message}");
                }
            }

            _memoryCache.Remove(key);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            if (_useRedis && _redis != null)
            {
                try
                {
                    var db = _redis.GetDatabase();
                    return await db.KeyExistsAsync(key);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro no Redis: {ex.Message}");
                }
            }

            return _memoryCache.TryGetValue(key, out _);
        }
    }
}