using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

namespace TaigadevDiscordBot.Core.Bot.Features.Commands
{
    public interface ICommand
    {
        string Command { get; }

        string Description { get; }

        public string UsageExample { get; }

        bool AuditCommand { get; }

        GuildPermission RequiredPermissions { get; }

        Task ExecuteAsync(SocketMessage message, SocketGuild guild);
    }
}