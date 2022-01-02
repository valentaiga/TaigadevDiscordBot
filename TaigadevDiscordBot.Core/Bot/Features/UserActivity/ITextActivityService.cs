using System.Threading.Tasks;

using TaigadevDiscordBot.Core.Bot.Event.EventArgs;

namespace TaigadevDiscordBot.Core.Bot.Features.UserActivity
{
    public interface ITextActivityService
    {
        ValueTask UpdateUserTextActivityAsync(NewTextMessageEventArgs eventArgs);

        ValueTask IncrementUserCookiesAsync(ulong userId, ulong guildId);
    }
}