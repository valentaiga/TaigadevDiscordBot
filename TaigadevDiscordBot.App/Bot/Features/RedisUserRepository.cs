using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using TaigadevDiscordBot.Core.Bot.Features;
using TaigadevDiscordBot.Core.Database.Redis;

namespace TaigadevDiscordBot.App.Bot.Features
{
    public class RedisUserRepository : IUserRepository
    {
        private readonly IRedisRepository _redisRepository;
        private readonly ILogger<RedisUserRepository> _logger;

        public RedisUserRepository(IRedisRepository redisRepository, ILogger<RedisUserRepository> logger)
        {
            _redisRepository = redisRepository;
            _logger = logger;
        }

        public async Task<User> GetOrCreateUserAsync(ulong userId, ulong guildId)
        {
            var user = await _redisRepository.GetAsync<User>(User.GetCacheKey(guildId, userId));

            if (user is null)
            {
                user = new User(guildId, userId);
                await _redisRepository.SaveAsync(user);
                _logger.LogDebug($"User with key '{user.GetCacheKey()}' created");
            }

            return user;
        }

        public async Task SaveUserAsync(User user)
        {
            await _redisRepository.SaveAsync(user);
            _logger.LogDebug($"User '{user.Nickname}' with key '{user.GetCacheKey()}' updated");
        }

        public async Task<User> UpdateUserAsync(ulong userId, ulong guildId, Func<User, Task> updateAction)
        {
            var user = await GetOrCreateUserAsync(userId, guildId);
            await updateAction(user);
            await SaveUserAsync(user);
            return user;
        }
    }
}