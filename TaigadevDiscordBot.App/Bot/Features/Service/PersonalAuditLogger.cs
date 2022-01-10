using System.Collections.Generic;
using System.Threading.Tasks;

using TaigadevDiscordBot.Core.Bot.Features.Service;
using TaigadevDiscordBot.Core.Database.Redis;

namespace TaigadevDiscordBot.App.Bot.Features.Service
{
    public class PersonalAuditLogger : IPersonalAuditLogger
    {
        private readonly IRedisProvider _redisProvider;

        public PersonalAuditLogger(IRedisProvider redisProvider)
        {
            _redisProvider = redisProvider;
        }

        public async Task AuditAsync(ulong userId, ulong guildId, string message)
        {
            if (await IsUserTrackedAsync(userId, guildId))
            {
                await _redisProvider.SetAddAsync(GetCacheKey(userId, guildId), message);
            }
        }

        public Task<IEnumerable<string>> GetPersonalLogsAsync(ulong userId, ulong guildId)
            => _redisProvider.SetGetAllAsync<string>(GetCacheKey(userId, guildId));

        public Task TrackUserAsync(ulong userId, ulong guildId)
            => _redisProvider.SetAddAsync(GetTrackedUsersKey(guildId), userId);

        private Task<bool> IsUserTrackedAsync(ulong userId, ulong guildId)
            => _redisProvider.SetContainsAsync(GetTrackedUsersKey(guildId), userId);

        private static string GetCacheKey(ulong userId, ulong guildId)
            => $"{guildId}:personalLogs:{userId}";

        private static string GetTrackedUsersKey(ulong guildId)
            => $"{guildId}:trackedUsers";
    }
}