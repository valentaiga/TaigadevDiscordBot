using System.Collections.Generic;
using System.Threading.Tasks;

using TaigadevDiscordBot.Core.Database.Redis;

namespace TaigadevDiscordBot.App.Database.Redis
{
    public class RedisRepository : IRedisRepository
    {
        private readonly IRedisProvider _redisProvider;

        public RedisRepository(IRedisProvider redisProvider)
        {
            // todo: add method to fetch all saved guild users (do I need it?)
            _redisProvider = redisProvider;
        }

        public Task<T> GetAsync<T>(string cacheKey) where T : IRedisEntity
        {
            return _redisProvider.GetAsync<T>(cacheKey);
        }

        public Task<T[]> GetAsync<T>(IAsyncEnumerable<string> cacheKeys) where T : IRedisEntity
        {
            return _redisProvider.GetAsync<T>(cacheKeys);
        }

        public Task SaveAsync<T>(T entity) where T : IRedisEntity
        {
            var cacheKey = entity.GetCacheKey();
            return _redisProvider.SaveAsync(cacheKey, entity);
        }
    }
}