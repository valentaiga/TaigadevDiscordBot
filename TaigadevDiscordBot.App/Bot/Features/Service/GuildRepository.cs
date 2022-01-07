using System;
using System.Threading.Tasks;

using TaigadevDiscordBot.Core.Bot.Features;
using TaigadevDiscordBot.Core.Bot.Features.Service;
using TaigadevDiscordBot.Core.Database.Redis;

namespace TaigadevDiscordBot.App.Bot.Features.Service
{
    public class GuildRepository : IGuildRepository
    {
        private readonly IRedisRepository _redisRepository;

        public GuildRepository(IRedisRepository redisRepository)
        {
            _redisRepository = redisRepository;
        }

        public async Task<Guild> GetGuildInformation(ulong guildId)
            => await _redisRepository.GetAsync<Guild>(Guild.GetCacheKey(guildId))
                ?? new Guild(guildId);

        public async Task UpdateGuildAsync(ulong guildId, Func<Guild, Task> updateAction)
        {
            var guild = await GetGuildInformation(guildId);
            await updateAction(guild);
        }
    }
}