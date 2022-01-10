using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaigadevDiscordBot.Core.Database.Redis
{
    public interface IRedisProvider
    {
        Task SaveAsync<T>(string cacheKey, T value);

        Task<T> GetAsync<T>(string cacheKey);

        Task<T[]> GetAsync<T>(IEnumerable<string> cacheKeys);

        Task AddToHashAsync<T>(string outerKey, string innerKey, T value);

        Task<Dictionary<string, TValue>> GetFromHashAllAsync<TValue>(string outerKey);

        Task<T> GetFromHashAsync<T>(string outerKey, string innerKey);

        Task SetAddAsync<T>(string cacheKey, T value);

        Task<IEnumerable<T>> SetGetAllAsync<T>(string cacheKey);

        Task<bool> SetContainsAsync<T>(string cacheKey, T value);

        Task SetAddAsync<T>(string cacheKey, IEnumerable<T> values);

        Task<IList<T>> PopSetAsync<T>(string cacheKey);
    }
}