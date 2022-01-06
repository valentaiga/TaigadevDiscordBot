using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;
using TaigadevDiscordBot.Core.Bot.Features.Commands;
using TaigadevDiscordBot.Core.Bot.Features.UserExperience;
using TaigadevDiscordBot.Core.Extensions;

namespace TaigadevDiscordBot.App.Bot.Features.Commands.Profile
{
    public class SetLevelCommand : CommandBase
    {
        private readonly IUserLevelService _userLevelService;

        public SetLevelCommand(IUserLevelService userLevelService)
            : base(
                "setlevel", 
                "Set leve of user", 
                "t!setlevel @mention 0", 
                true, 
                GuildPermission.Administrator)
        {
            _userLevelService = userLevelService;
        }

        public override async Task ExecuteAsync(SocketMessage message, SocketGuild guild)
        {
            if (message.MentionedUsers.FirstOrDefault() is not SocketGuildUser mentionedUser)
            {
                await message.CommandMessageReplyAsync($"Command '{Command}' requires user to mention.");
                return;
            }

            var split = message.Content.Split(' ');
            if (split.Length != 3 || !int.TryParse(split[2], out var setLevel))
            {
                await message.CommandMessageReplyAsync($"Command '{Command}' incorrect usage. Example: '{UsageExample}'");
                return;
            }
            
            var user = await _userLevelService.SetUserLevelAsync(mentionedUser, setLevel);
        }
    }
}