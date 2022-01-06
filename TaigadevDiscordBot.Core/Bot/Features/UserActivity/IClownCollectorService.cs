using System.Collections.Generic;
using System.Threading.Tasks;

using TaigadevDiscordBot.Core.Bot.Event.EventArgs;

namespace TaigadevDiscordBot.Core.Bot.Features.UserActivity
{
    public interface IClownCollectorService
    {
        ValueTask IncrementUpdateUserClowns(ReactionAddedEventArgs eventArgs);

        Task<int> GetCurrentUserCount(ulong userId, ulong guildId);

        Task<Dictionary<ulong, int>> GetCurrentGuildTop(ulong guildId);
    }
}