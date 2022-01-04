using System.Threading.Tasks;

using Discord.WebSocket;

namespace TaigadevDiscordBot.Core.Bot.Features.Commands
{
    public interface ICommand
    {
        string Command { get; }

        string Description { get; }

        public string UsageExample { get; }

        bool AuditCommand { get; }

        Task ExecuteAsync(SocketMessage message, SocketGuild guild);
    }
}