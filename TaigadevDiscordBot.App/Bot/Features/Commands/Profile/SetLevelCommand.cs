using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

using Microsoft.Extensions.Logging;

using TaigadevDiscordBot.Core.Bot;
using TaigadevDiscordBot.Core.Bot.Features;
using TaigadevDiscordBot.Core.Bot.Features.Commands;
using TaigadevDiscordBot.Core.Bot.Features.Service;
using TaigadevDiscordBot.Core.Bot.Features.UserExperience;
using TaigadevDiscordBot.Core.Extensions;

namespace TaigadevDiscordBot.App.Bot.Features.Commands.Profile
{
    public class SetLevelCommand : CommandBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserLevelService _userLevelService;
        private readonly ILogger<SetLevelCommand> _logger;
        private readonly IAuditLogger _auditLogger;

        public SetLevelCommand(
            IBotConfiguration botConfiguration,
            IUserRepository userRepository, 
            IUserLevelService userLevelService, 
            ILogger<SetLevelCommand> logger, 
            IAuditLogger auditLogger)
            : base(
                "setlevel", 
                "Change mentioned user level", 
                $"{botConfiguration.Prefix}setlevel @mention 0", 
                true, 
                GuildPermission.Administrator)
        {
            _userRepository = userRepository;
            _userLevelService = userLevelService;
            _logger = logger;
            _auditLogger = auditLogger;
        }

        public override async Task ExecuteAsync(SocketMessage message, SocketGuild guild)
        {
            var mentionedSocketUser = message.MentionedUsers.FirstOrDefault();
            if (mentionedSocketUser is null)
            {
                await message.CommandMessageReplyAsync($"Command '{Command}' requires user to mention. Example: '{UsageExample}'");
                return;
            }

            var split = message.Content.Split(' ');
            if (split.Length != 3 || !int.TryParse(split[2], out var setLevel))
            {
                await message.CommandMessageReplyAsync($"Command '{Command}' incorrect usage. Example: '{UsageExample}'");
                return;
            }
            
            var mentionedUser = guild.GetUser(mentionedSocketUser.Id);

            if (mentionedUser is null)
            {
                await message.CommandMessageReplyAsync($"Command is temporarily unavailable for mentioned user.");
                return;
            }

            var user = await _userRepository.GetOrCreateUserAsync(mentionedUser.Id, guild.Id);
            var embedMessageFields = new Dictionary<string, string>
            {
                { "Previous level", user.Level.ToString() },
                { "Previous experience", user.Experience.ToString() }
            };
            user = await _userLevelService.SetUserLevelAsync(mentionedUser, setLevel);
            var auditMessage = $"User '{user.Nickname}' with id '{user.UserId}' level set to '{setLevel}' by '{message.Author.Username}' with id '{message.Author.Id}'";
            embedMessageFields.Add("Current level", user.Level.ToString());
            embedMessageFields.Add("Current experience", user.Experience.ToString());
            
            _logger.LogDebug(auditMessage);
            await _auditLogger.LogInformationAsync(auditMessage, guild.Id, embedMessageFields);
        }
    }
}