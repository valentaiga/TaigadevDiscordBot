using System.Collections.Generic;
using System.Threading.Tasks;

using TaigadevDiscordBot.Core.Bot.Event.EventArgs;

namespace TaigadevDiscordBot.Core.Bot.Features.Collectors
{
    public interface IEmojiCounterService
    {
        ValueTask IncrementUserEmojiCount(ReactionAddedEventArgs eventArgs);

        Task<int> GetCurrentUserCount(ulong userId, ulong guildId, string emoji);

        Task<Dictionary<ulong, int>> GetCurrentGuildTop(ulong guildId, string emoji);
    }
}