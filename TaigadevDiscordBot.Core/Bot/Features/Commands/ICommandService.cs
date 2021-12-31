using System.Threading.Tasks;

using TaigadevDiscordBot.Core.Bot.Event.EventArgs;

namespace TaigadevDiscordBot.Core.Bot.Features.Commands
{
    public interface ICommandService
    {
        ValueTask ExecuteCommandAsync(NewTextMessageEventArgs eventArgs);
    }
}