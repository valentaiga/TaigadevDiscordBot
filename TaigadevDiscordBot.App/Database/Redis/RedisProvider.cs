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
            await Database.StringSetAsync(AdjustProjectPrefix(cacheKey), json);
        }

        public async Task<T> GetFromHashAsync<T>(string outerKey, string innerKey)
        {
            var result = await Database.HashGetAsync(AdjustProjectPrefix(outerKey), innerKey);
            return DeserializeValue<T>(result);
        }

        public async Task<Dictionary<string, TValue>> GetFromHashAllAsync<TValue>(string outerKey)
        {
            var result = await Database.HashGetAllAsync(AdjustProjectPrefix(outerKey));
            return result.ToDictionary(x => x.Name.ToString(), x => DeserializeValue<TValue>(x.Value));
        }

        public Task AddToHashAsync<T>(string outerKey, string innerKey, T value)
        { 
            return Database.HashSetAsync(AdjustProjectPrefix(outerKey), innerKey, SerializeValue(value));
        }

        public Task SetAddAsync<T>(string cacheKey, T value)
        {
            return Database.SetAddAsync(AdjustProjectPrefix(cacheKey), SerializeValue(value));
        }

        public Task<bool> SetContainsAsync<T>(string cacheKey, T value)
        {
            return Database.SetContainsAsync(AdjustProjectPrefix(cacheKey), SerializeValue(value));
        }

        public Task SetAddAsync<T>(string cacheKey, IEnumerable<T> values)
        {
            var redisValues = values.Select(x => new RedisValue(SerializeValue(x))).ToArray();
            return Database.SetAddAsync(AdjustProjectPrefix(cacheKey), redisValues);
        }

        public async Task<IEnumerable<T>> SetGetAllAsync<T>(string cacheKey)
        {
            var result = await Database.SetMembersAsync(AdjustProjectPrefix(cacheKey));
            return result is null ? Enumerable.Empty<T>() : result.Select(x => DeserializeValue<T>(x));
        }

        public async Task<IList<T>> PopSetAsync<T>(string cacheKey)
        {
            var result = new List<T>();
            RedisValue value;

            while ((value = await Database.SetPopAsync(AdjustProjectPrefix(cacheKey))).HasValue)
            {
                result.Add(DeserializeValue<T>(value));
            }

            return result;
        }

        public async Task<T> GetAsync<T>(string cacheKey)
        {
            var result = await Database.StringGetAsync(AdjustProjectPrefix(cacheKey));
            if (result.HasValue)
            {
                return DeserializeValue<T>(result);
            }

            return default;
        }

        public async Task<T[]> GetAsync<T>(IEnumerable<string> cacheKeys)
        {
            var values = await Database.StringGetAsync(cacheKeys.Select(x => (RedisKey)AdjustProjectPrefix(x)).ToArray());
            return values
                .Where(x => x.HasValue)
                .Select(x => DeserializeValue<T>(x))
                .ToArray();
        }

        public void Dispose()
        {
            _multiplexer?.Dispose();
        }

        private IDatabase Database => _multiplexer.GetDatabase();

        private string AdjustProjectPrefix(string cacheKey) => $"{_prefix}:{cacheKey}";

        private string SerializeValue<T>(T value) => JsonSerializer.Serialize(value, _jsonSerializerOptions);

        private T DeserializeValue<T>(string json) => json is not null ? JsonSerializer.Deserialize<T>(json, _jsonSerializerOptions) : default;
    }
}