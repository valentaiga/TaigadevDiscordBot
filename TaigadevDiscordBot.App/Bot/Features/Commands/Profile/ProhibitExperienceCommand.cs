using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

using TaigadevDiscordBot.Core.Bot;
using TaigadevDiscordBot.Core.Bot.Features.Commands;
using TaigadevDiscordBot.Core.Bot.Features.UserExperience;
using TaigadevDiscordBot.Core.Extensions;

namespace TaigadevDiscordBot.App.Bot.Features.Commands.Profile
{
    public class ProhibitExperienceCommand : CommandBase
    {
        private readonly IExperienceService _experienceService;

        public ProhibitExperienceCommand(IBotConfiguration botConfiguration, IExperienceService experienceService) 
            : base(
                "prohibit", 
                "Prohibit user experience gain from all sources", 
                $"{botConfiguration.Prefix}prohibit @mention", 
                true,
                GuildPermission.Administrator)
        {
            _experienceService = experienceService;
        }

        public override async Task ExecuteAsync(SocketMessage message, IGuild dsGuild)
        {
            var mentionedSocketUser = message.MentionedUsers.FirstOrDefault();
            if (mentionedSocketUser is null)
            {
                await message.CommandMessageReplyAsync($"Command '{Command}' requires user to mention. Example: '{UsageExample}'");
                return;
            }

            await _experienceService.ProhibitExperienceGainAsync(mentionedSocketUser.Id, dsGuild.Id);
            await message.CommandMessageReplyAsync($"User successfully prohibited from experience gain");
        }
    }
}