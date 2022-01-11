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
    public class AllowExperienceGainCommand : CommandBase
    {
        private readonly IExperienceService _experienceService;

        public AllowExperienceGainCommand(IBotConfiguration botConfiguration, IExperienceService experienceService)
            : base(
                "allowexp",
                "Allow user experience gain from all sources",
                $"{botConfiguration.Prefix}allowexp @mention",
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

            await _experienceService.AllowExperienceGainAsync(mentionedSocketUser.Id, dsGuild.Id);
            await message.CommandMessageReplyAsync($"The user is allowed to gain experience");
        }
    }
}