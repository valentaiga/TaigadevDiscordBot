using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Discord.WebSocket;

using TaigadevDiscordBot.Core.Bot.Event.EventArgs;
using TaigadevDiscordBot.Core.Bot.Features.Commands;

namespace TaigadevDiscordBot.App.Bot.Features.Commands
{
    public class CommandService : ICommandService
    {
        private readonly string _prefix;
        private readonly ConcurrentDictionary<string, ITextChannelCommand> _textCommands;

        public CommandService(IEnumerable<ITextChannelCommand> textCommands)
        {
            _prefix = "t!";
            _textCommands = new(textCommands.ToDictionary(x => x.Command, x => x));
            // todo: add prefix to config and check for prefix in the beginnign of message
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
                // todo: add required permissions for commands
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