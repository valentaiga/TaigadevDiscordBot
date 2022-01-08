using System;
using System.Threading.Tasks;

namespace TaigadevDiscordBot.Core.Bot.Features.Service
{
    public interface IGuildRepository
    {
        Task<Guild> GetGuildAsync(ulong guildId);

        Task SaveGuildAsync(Guild guild);

        Task UpdateGuildAsync(ulong guildId, Func<Guild, Task> updateAction);
    }
}