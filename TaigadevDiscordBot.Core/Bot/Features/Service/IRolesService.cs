using System.Threading.Tasks;

using TaigadevDiscordBot.Core.Bot.Event.EventArgs;

namespace TaigadevDiscordBot.Core.Bot.Features.Service
{
    public interface IRolesService
    {
        ValueTask SetRolesToNewUser(UserJoinedEventArgs eventArgs);
    }
}