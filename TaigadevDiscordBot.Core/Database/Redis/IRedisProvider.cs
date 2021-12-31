using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaigadevDiscordBot.Core.Database.Redis
{
    public interface IRedisProvider
    {
        Task SaveAsync<T>(string cacheKey, T value);

        Task<T> GetAsync<T>(string cacheKey);

        Task<T[]> GetAsync<T>(IAsyncEnumerable<string> cacheKeys);
    }
}