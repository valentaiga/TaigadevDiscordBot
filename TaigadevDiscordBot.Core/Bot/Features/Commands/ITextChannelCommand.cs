using System.Threading.Tasks;

using Discord.WebSocket;

namespace TaigadevDiscordBot.Core.Bot.Features.Commands
{
    public interface ITextChannelCommand : ICommand
    {
        Task ExecuteAsync(SocketMessage message, SocketGuild guild);
    }
}