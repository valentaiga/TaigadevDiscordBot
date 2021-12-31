using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaigadevDiscordBot.Core.Database.Redis
{
    public interface IRedisRepository
    {
        Task<T> GetAsync<T>(string cacheKey) where T : IRedisEntity;

        Task<T[]> GetAsync<T>(IAsyncEnumerable<string> cacheKeys) where T : IRedisEntity;

        Task SaveAsync<T>(T entity) where T : IRedisEntity;
    }
}