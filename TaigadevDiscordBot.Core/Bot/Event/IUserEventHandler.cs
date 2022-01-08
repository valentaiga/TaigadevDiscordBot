using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

namespace TaigadevDiscordBot.Core.Bot.Event
{
    public interface IUserEventHandler
    {
        Task OnUserVoiceStateUpdated(SocketUser user, SocketVoiceState oldVoiceState, SocketVoiceState newVoiceState);
        Task OnMessageReceived(SocketMessage message);
        Task OnReactionAdded(Cacheable<IUserMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> cachedTextChannel, SocketReaction reaction);
        Task OnUserJoined(SocketGuildUser dsUser);
    }
}