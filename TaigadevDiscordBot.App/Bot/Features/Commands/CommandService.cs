using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Discord.WebSocket;

using TaigadevDiscordBot.Core.Bot.Event.EventArgs;
using TaigadevDiscordBot.Core.Bot.Features.Commands;
using TaigadevDiscordBot.Core.Bot.Features.Service;
using TaigadevDiscordBot.Core.Extensions;

namespace TaigadevDiscordBot.App.Bot.Features.Commands
{
    public class CommandService : ICommandService
    {
        private readonly string _prefix;
        private readonly ConcurrentDictionary<string, ICommand> _textCommands;
        private readonly IAuditLogger _auditLogger;

        public CommandService(IEnumerable<ICommand> textCommands, IAuditLogger auditLogger)
        {
            _auditLogger = auditLogger;
            _prefix = "t!";
            _textCommands = new(textCommands.ToDictionary(x => x.Command, x => x));
        }

        private bool IsCommandCall(SocketMessage message)
            => message.Content.StartsWith(_prefix);

        public async ValueTask ExecuteCommandAsync(NewTextMessageEventArgs eventArgs)
        {
            if (!IsCommandCall(eventArgs.Message))
            {
                return;
            }

            var textCommand = GetCommandFromMessage(eventArgs.Message);

            if (_textCommands.TryGetValue(textCommand, out var command))
            {
                var userPermissions = eventArgs.User.GuildPermissions.ToList();
                if (!userPermissions.Contains(command.RequiredPermissions))
                {
                    await eventArgs.Message.CommandMessageReplyAsync($"Not enough permissions to use this command, {eventArgs.User.Mention}. {command.RequiredPermissions} is required");
                    return;
                }

                await _auditLogger.LogInformationAsync(
                    null,
                    eventArgs.Guild.Id,
                    new Dictionary<string, string>
                    {
                        { "Author", eventArgs.User.Mention },
                        { "Command", textCommand },
                        { "Mentioned", eventArgs.Message.MentionedUsers.Count > 0 ? string.Join(", ", eventArgs.Message.MentionedUsers) : "<none>" },
                        { "Message", eventArgs.Message.Content}
                    });
                // todo: use slash with prefix instead of just prefix in chat - https://labs.discordnet.dev/guides/int_basics/application-commands/slash-commands/creating-slash-commands.html
                await command.ExecuteAsync(eventArgs.Message, eventArgs.Guild);
            }
        }

        private string GetCommandFromMessage(SocketMessage message)
        {
            var split = message.Content.Trim().Split(" ");

            if (split.Length == 0 || split[0].Length <= _prefix.Length)
            {
                return null;
            }

            return split[0][_prefix.Length..];
        }
    }
}