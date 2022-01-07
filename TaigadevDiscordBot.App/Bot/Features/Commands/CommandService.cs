using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Discord.WebSocket;
using TaigadevDiscordBot.Core.Bot;
using TaigadevDiscordBot.Core.Bot.Event.EventArgs;
using TaigadevDiscordBot.Core.Bot.Features.Commands;
using TaigadevDiscordBot.Core.Bot.Features.Service;
using TaigadevDiscordBot.Core.Extensions;

namespace TaigadevDiscordBot.App.Bot.Features.Commands
{
    public class CommandService : ICommandService
    {
        private readonly ConcurrentDictionary<string, ICommand> _textCommands;
        private readonly IBotConfiguration _botConfiguration;
        private readonly IGuildRepository _guildRepository;
        private readonly IAuditLogger _auditLogger;

        public CommandService(IBotConfiguration botConfiguration, IGuildRepository guildRepository, IEnumerable<ICommand> textCommands, IAuditLogger auditLogger)
        {
            _botConfiguration = botConfiguration;
            _guildRepository = guildRepository;
            _auditLogger = auditLogger;
            _textCommands = new(textCommands.ToDictionary(x => x.Command, x => x));
        }

        private bool IsCommandCall(SocketMessage message)
            => message.Content.StartsWith(_botConfiguration.Prefix);

        public async ValueTask ExecuteCommandAsync(NewTextMessageEventArgs eventArgs)
        {
            if (!IsCommandCall(eventArgs.Message))
            {
                return;
            }

            var textCommand = GetCommandFromMessage(eventArgs.Message);

            if (!_textCommands.TryGetValue(textCommand, out var command))
            {
                if (_textCommands.TryGetValue("help", out var helpCommand))
                {
                    await eventArgs.Message.CommandMessageReplyAsync(
                        $"Unknown command. Find available commands by '{helpCommand.UsageExample}' usage");
                }
                return;
            }

            if (await IsTextChannelIgnoredAsync(eventArgs.Message.Channel.Id, eventArgs.Guild.Id)
                && !command.AllowToUseInIgnore)
            {
                if (_textCommands.TryGetValue("unignore", out var unignoreCommand))
                {
                    await eventArgs.Message.CommandMessageReplyAsync(
                        $"Text chat is ignored and commands cant be executed here. You can unignore this channel by using '{unignoreCommand.UsageExample}' command");
                }
                 return;   
            }

            var userPermissions = eventArgs.User.GuildPermissions.ToList();
            if (!userPermissions.Contains(command.RequiredPermissions))
            {
                await eventArgs.Message.CommandMessageReplyAsync($"Not enough permissions to use this command, {eventArgs.User.Mention}. {command.RequiredPermissions} is required");
                return;
            }

            if (command.AuditCommand)
            {
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
            }
            
            // todo: use slash with prefix instead of just prefix in chat - https://labs.discordnet.dev/guides/int_basics/application-commands/slash-commands/creating-slash-commands.html
            await command.ExecuteAsync(eventArgs.Message, eventArgs.Guild);
        }

        private string GetCommandFromMessage(SocketMessage message)
        {
            var split = message.Content.Trim().Split(" ");

            if (split.Length == 0)
            {
                return null;
            }

            return split[0][_botConfiguration.Prefix.Length..];
        }

        private async Task<bool> IsTextChannelIgnoredAsync(ulong textChannelId, ulong guildId)
        {
            var guild = await _guildRepository.GetGuildAsync(guildId);
            return guild.IgnoredChannels.Contains(textChannelId);
        }
    }
}