using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

using TaigadevDiscordBot.Core.Bot;
using TaigadevDiscordBot.Core.Bot.Features.Commands;
using TaigadevDiscordBot.Core.Bot.Features.Service;
using TaigadevDiscordBot.Core.Extensions;

namespace TaigadevDiscordBot.App.Bot.Features.Commands.Unspecified
{
    public class TrackUserCommand : CommandBase
    {
        private readonly IPersonalAuditLogger _personalAuditLogger;

        public TrackUserCommand(IBotConfiguration botConfiguration, IPersonalAuditLogger personalAuditLogger)
            : base(
                "spersaudit",
                "Start user personal audit collecting",
                $"{botConfiguration.Prefix}spersaudit",
                true,
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

            await _personalAuditLogger.TrackUserAsync(mentionedUser.Id, dsGuild.Id);
            await message.CommandMessageReplyAsync($"Personal track on user '{mentionedUser.Username}' started");
        }
    }
}