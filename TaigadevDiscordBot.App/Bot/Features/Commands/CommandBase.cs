using System.Threading.Tasks;

using Discord.WebSocket;

using TaigadevDiscordBot.Core.Bot.Features.Commands;

namespace TaigadevDiscordBot.App.Bot.Features.Commands
{
    public abstract class CommandBase : ICommand
    {
        protected CommandBase(string command, string description, string usageExample, bool auditCommand)
        {
            Command = command;
            Description = description;
            UsageExample = usageExample;
            AuditCommand = auditCommand;
        }

        public string Command { get; }

        public string Description { get; }

        public string UsageExample { get; }

        public bool AuditCommand { get; }

        public abstract Task ExecuteAsync(SocketMessage message, SocketGuild guild);
    }
}