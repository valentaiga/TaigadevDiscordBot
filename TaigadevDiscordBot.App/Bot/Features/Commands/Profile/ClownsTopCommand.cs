using System;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;
using TaigadevDiscordBot.Core.Bot;
using TaigadevDiscordBot.Core.Bot.Features;
using TaigadevDiscordBot.Core.Bot.Features.Commands;
using TaigadevDiscordBot.Core.Bot.Features.UserActivity;
using TaigadevDiscordBot.Core.Constants;
using TaigadevDiscordBot.Core.Extensions;
using TaigadevDiscordBot.Core.Helpers;

namespace TaigadevDiscordBot.App.Bot.Features.Commands.Profile
{
    public class ClownsTopCommand : CommandBase
    {
        private readonly IClownCollectorService _clownCollectorService;
        private readonly IUserRepository _userRepository;
        private readonly IBotConfiguration _botConfiguration;

        public ClownsTopCommand(IClownCollectorService clownCollectorService, IUserRepository userRepository, IBotConfiguration botConfiguration) 
            : base(
                "topclowns", 
                "Get a current guild clown top", 
                "t!topclowns", 
                false, 
                GuildPermission.SendMessages)
        {
            _clownCollectorService = clownCollectorService;
            _userRepository = userRepository;
            _botConfiguration = botConfiguration;
        }

        public override async Task ExecuteAsync(SocketMessage message, SocketGuild guild)
        {
            var top = await _clownCollectorService.GetCurrentGuildTop(guild.Id);
            var embedMessage = new EmbedBuilder()
                .WithTitle($"'{guild.Name}' top clowns {Emojis.ClownEmote}")
                .AdjustBotFields(_botConfiguration);

            var index = 1;
            foreach (var keyValuePair in top)
            {
                var dsUser = guild.GetUser(keyValuePair.Key);
                var user = await _userRepository.GetOrCreateUserAsync(keyValuePair.Key, guild.Id);
                var nickname = dsUser?.Nickname ?? dsUser?.Username ?? user.Nickname;
                embedMessage.AddField($"#{index++}. {nickname}", BeautifyHelper.GetUserInfo(user, keyValuePair.Value, -1));
            }

            await message.CommandMessageReplyAsync(null, embedMessage.Build(), TimeSpan.MaxValue);
        }
    }
}