using System.Threading.Tasks;

using Discord.WebSocket;

namespace TaigadevDiscordBot.Core.Bot.Event
{
    public interface IUserEventHandler
    {
        Task OnUserVoiceStateUpdated(SocketUser user, SocketVoiceState oldVoiceState, SocketVoiceState newVoiceState);
        Task OnMessageReceived(SocketMessage message);
    }
}