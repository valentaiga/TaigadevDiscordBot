using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using StackExchange.Redis;

using TaigadevDiscordBot.Core.Database.Redis;

namespace TaigadevDiscordBot.App.Database.Redis
{
    public class RedisProvider : IRedisProvider, IDisposable
    {
        private readonly ConnectionMultiplexer _multiplexer;
        private readonly string _prefix;

        public RedisProvider(IRedisConfiguration redisConfiguration)
        {
            _prefix = redisConfiguration.Prefix;

            var redisOptions = new ConfigurationOptions()
            {
                EndPoints =
                {
                    redisConfiguration.Endpoint
                },
                Password = redisConfiguration.Password
            };
            _multiplexer = ConnectionMultiplexer.Connect(redisOptions);
        }
        
        public async Task SaveAsync<T>(string cacheKey, T value)
        {
            var json = JsonSerializer.Serialize(value);
            var db = GetDatabase();
            await db.StringSetAsync(AdjustProjectPrefix(cacheKey), json);
        }

        public async Task<T> GetAsync<T>(string cacheKey)
        {
            var db = GetDatabase();
            var result = await db.StringGetAsync(AdjustProjectPrefix(cacheKey));
            if (result.HasValue)
            {
                return JsonSerializer.Deserialize<T>(result);
            }

            return default;
        }

        public async Task<T[]> GetAsync<T>(IAsyncEnumerable<string> cacheKeys)
        {
            var db = GetDatabase();
            var values = await db.StringGetAsync(await cacheKeys.Select(x => (RedisKey)AdjustProjectPrefix(x)).ToArrayAsync());
            return await values.ToAsyncEnumerable()
                .Where(x => x.HasValue)
                .Select(x => JsonSerializer.Deserialize<T>(x))
                .ToArrayAsync();
        }

        public void Dispose()
        {
            _multiplexer?.Dispose();
        }

        private IDatabase GetDatabase() => _multiplexer.GetDatabase();

        private string AdjustProjectPrefix(string cacheKey) => $"{_prefix}:{cacheKey}";
    }
}