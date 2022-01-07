using System;
using System.Threading.Tasks;

namespace TaigadevDiscordBot.Core.Bot.Features.Service
{
    public interface IGuildRepository
    {
        Task<Guild> GetGuildInformation(ulong guildId);

        Task UpdateGuildAsync(ulong guildId, Func<Guild, Task> updateAction);
    }
}