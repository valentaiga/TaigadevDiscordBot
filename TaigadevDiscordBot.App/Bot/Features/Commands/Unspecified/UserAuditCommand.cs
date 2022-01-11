using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

using TaigadevDiscordBot.Core.Bot;
using TaigadevDiscordBot.Core.Bot.Features.Commands;
using TaigadevDiscordBot.Core.Bot.Features.Service;
using TaigadevDiscordBot.Core.Extensions;

namespace TaigadevDiscordBot.App.Bot.Features.Commands.Unspecified
{
    public class UserAuditCommand : CommandBase
    {
        private readonly IPersonalAuditLogger _personalAuditLogger;

        public UserAuditCommand(IBotConfiguration botConfiguration, IPersonalAuditLogger personalAuditLogger) 
            : base(
                "persaudit", 
                "Get personal audit", 
                $"{botConfiguration.Prefix}persaudit", 
                false, 
                GuildPermission.Administrator)
        {
            _personalAuditLogger = personalAuditLogger;
        }

        public override async Task ExecuteAsync(SocketMessage message, IGuild dsGuild)
        {
            var mentionedUser = message.MentionedUsers.FirstOrDefault();

            if (mentionedUser is null)
            {
                await message.CommandMessageReplyAsync($"Command '{Command}' requires user to mention. Example: '{UsageExample}'");
                return;
            }

            var personalLogs = await _personalAuditLogger.GetPersonalLogsAsync(mentionedUser.Id, dsGuild.Id);
            var sb = new StringBuilder();
            sb.Append("```");

            var index = 0;
            foreach (var log in personalLogs)
            {
                sb.AppendLine($"{index++}: {log}");
            }

            if (index == 0)
            {
                sb.AppendLine("- no logs found -");
            }
            sb.Append("```");

            await message.CommandMessageReplyAsync(sb.ToString(), deleteMessageTimespan: TimeSpan.FromMinutes(1));
        }
    }
}