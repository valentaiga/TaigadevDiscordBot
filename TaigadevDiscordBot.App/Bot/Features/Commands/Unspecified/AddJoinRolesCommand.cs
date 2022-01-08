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
    public class AddJoinRolesCommand : CommandBase
    {
        private readonly IGuildRepository _guildRepository;

        public AddJoinRolesCommand(IBotConfiguration botConfiguration, IGuildRepository guildRepository) 
            : base(
                "addjoinroles", 
                "Add roles for newly joined to guild users", 
                $"{botConfiguration.Prefix}addjoinroles @role_mention @role_mention", 
                true, 
                GuildPermission.Administrator)
        {
            _guildRepository = guildRepository;
        }

        public override async Task ExecuteAsync(SocketMessage message, SocketGuild dsGuild)
        {
            var mentionedRoles = message.MentionedRoles;
            var guild = await _guildRepository.GetGuildAsync(dsGuild.Id);

            var anyRoleAdded = false;
            foreach (var roleId in mentionedRoles.Select(x => x.Id))
            {
                if (!guild.DefaultRoles.Contains(roleId))
                {
                    anyRoleAdded = true;
                    guild.DefaultRoles.Add(roleId);
                }
            }

            await _guildRepository.SaveGuildAsync(guild);

            var replyText = anyRoleAdded
                ? $"Roles successfully added as default. "
                : $"No new roles added as default. ";
            replyText += $"Current default roles: [{string.Join(", ", mentionedRoles.Select(x => x.Name))}]";
            await message.CommandMessageReplyAsync(replyText);
        }
    }
}