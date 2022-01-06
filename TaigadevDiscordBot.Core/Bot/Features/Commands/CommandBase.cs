using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

namespace TaigadevDiscordBot.Core.Bot.Features.Commands
{
    public abstract class CommandBase : ICommand
    {
        protected CommandBase(string command, string description, string usageExample, bool auditCommand, GuildPermission requiredPermissions)
        {
            Command = command;
            Description = description;
            UsageExample = usageExample;
            AuditCommand = auditCommand;
            RequiredPermissions = requiredPermissions;
        }

        public string Command { get; }

        public string Description { get; }

        public string UsageExample { get; }

        public bool AuditCommand { get; }

        public GuildPermission RequiredPermissions { get; }

        public abstract Task ExecuteAsync(SocketMessage message, SocketGuild guild);
    }
}