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
    public class RemoveJoinRolesCommand : CommandBase
    {
        private readonly IGuildRepository _guildRepository;

        public RemoveJoinRolesCommand(IBotConfiguration botConfiguration, IGuildRepository guildRepository)
            : base(
                "removejoinroles",
                "Remove roles for newly joined to guild users",
                $"{botConfiguration.Prefix}removejoinroles @role_mention @role_mention",
                true,
                GuildPermission.Administrator)
        {
            _guildRepository = guildRepository;
        }

        public override async Task ExecuteAsync(SocketMessage message, IGuild dsGuild)
        {
            var mentionedRoles = message.MentionedRoles;
            var guild = await _guildRepository.GetGuildAsync(dsGuild.Id);

            var anyRoleRemoved = false;
            foreach (var roleId in mentionedRoles.Select(x => x.Id))
            {
                if (guild.DefaultRoles.Contains(roleId))
                {
                    anyRoleRemoved = true;
                    guild.DefaultRoles.Remove(roleId);
                }
            }

            await _guildRepository.SaveGuildAsync(guild);

            var replyText = anyRoleRemoved
                ? $"Roles successfully removed. "
                : $"No roles removed. ";
            replyText += $"Current default roles: [{string.Join(", ", mentionedRoles.Select(x => x.Name))}]";
            await message.CommandMessageReplyAsync(replyText);
        }
    }
}