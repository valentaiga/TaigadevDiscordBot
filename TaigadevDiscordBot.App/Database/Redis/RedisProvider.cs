using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

using StackExchange.Redis;

using TaigadevDiscordBot.Core.Database.Redis;

namespace TaigadevDiscordBot.App.Database.Redis
{
    public class RedisProvider : IRedisProvider, IDisposable
    {
        private readonly ConnectionMultiplexer _multiplexer;
        private readonly string _prefix;

        private readonly JsonSerializerOptions _jsonSerializerOptions = new ()
        {
            WriteIndented = false,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.Cyrillic, UnicodeRanges.BasicLatin)
        };

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
            var json = SerializeValue(value);
            var db = GetDatabase();
            await db.StringSetAsync(AdjustProjectPrefix(cacheKey), json);
        }

        public Task AddToListAsync<T>(string cacheKey, IEnumerable<T> values)
        {
            var db = GetDatabase();
            var redisValues = values.Select(x => new RedisValue(SerializeValue(x))).ToArray();
            return db.SetAddAsync(cacheKey, redisValues);
        }

        public async Task<IList<T>> GetListAsync<T>(string cacheKey)
        {
            var db = GetDatabase();
            var result = new List<T>();
            RedisValue value;
            
            while ((value = await db.SetPopAsync(cacheKey)).HasValue)
            {
                result.Add(DeserializeValue<T>(value));
            }

            return result;
        }

        public async Task<T> GetAsync<T>(string cacheKey)
        {
            var db = GetDatabase();
            var result = await db.StringGetAsync(AdjustProjectPrefix(cacheKey));
            if (result.HasValue)
            {
                return DeserializeValue<T>(result);
            }

            return default;
        }

        public async Task<T[]> GetAsync<T>(IAsyncEnumerable<string> cacheKeys)
        {
            var db = GetDatabase();
            var values = await db.StringGetAsync(await cacheKeys.Select(x => (RedisKey)AdjustProjectPrefix(x)).ToArrayAsync());
            return await values.ToAsyncEnumerable()
                .Where(x => x.HasValue)
                .Select(x => DeserializeValue<T>(x))
                .ToArrayAsync();
        }

        public void Dispose()
        {
            _multiplexer?.Dispose();
        }

        private IDatabase GetDatabase() => _multiplexer.GetDatabase();

        private string AdjustProjectPrefix(string cacheKey) => $"{_prefix}:{cacheKey}";

        private string SerializeValue<T>(T value) => JsonSerializer.Serialize(value, _jsonSerializerOptions);

        private T DeserializeValue<T>(string json) => JsonSerializer.Deserialize<T>(json, _jsonSerializerOptions);
    }
}