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

        public async Task<Guild> GetGuildAsync(ulong guildId)
            => await _redisRepository.GetAsync<Guild>(Guild.GetCacheKey(guildId))
                ?? new Guild(guildId);

        public Task SaveGuildAsync(Guild guild)
            => _redisRepository.SaveAsync(guild);

        public async Task UpdateGuildAsync(ulong guildId, Func<Guild, Task> updateAction)
        {
            var guild = await GetGuildAsync(guildId);
            await updateAction(guild);
            await _redisRepository.SaveAsync(guild);
        }
    }
}