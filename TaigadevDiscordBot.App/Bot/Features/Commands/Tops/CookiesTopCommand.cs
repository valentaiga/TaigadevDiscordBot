using System;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

using TaigadevDiscordBot.Core.Bot;
using TaigadevDiscordBot.Core.Bot.Features;
using TaigadevDiscordBot.Core.Bot.Features.Collectors;
using TaigadevDiscordBot.Core.Bot.Features.Commands;
using TaigadevDiscordBot.Core.Constants;
using TaigadevDiscordBot.Core.Extensions;
using TaigadevDiscordBot.Core.Helpers;

namespace TaigadevDiscordBot.App.Bot.Features.Commands.Tops
{
    public class CookiesTopCommand : CommandBase
    {
        private readonly IEmojiCounterService _emojiCounterService;
        private readonly IUserRepository _userRepository;
        private readonly IBotConfiguration _botConfiguration;

        public CookiesTopCommand(
            IEmojiCounterService emojiCounterService,
            IUserRepository userRepository,
            IBotConfiguration botConfiguration)
            : base(
                "topcookies",
                "Get a current guild cookie top",
                $"{botConfiguration.Prefix}topcookies",
                false,
                GuildPermission.SendMessages)
        {
            _emojiCounterService = emojiCounterService;
            _userRepository = userRepository;
            _botConfiguration = botConfiguration;
        }

        public override async Task ExecuteAsync(SocketMessage message, IGuild dsGuild)
        {
            var top = await _emojiCounterService.GetCurrentGuildTop(dsGuild.Id, Emojis.CookieEmote);
            var embedMessage = new EmbedBuilder()
                .WithTitle($"'{dsGuild.Name}' top cookie holders {Emojis.CookieEmote}")
                .AdjustBotFields(_botConfiguration);

            var index = 1;
            foreach (var keyValuePair in top)
            {
                var dsUser = await dsGuild.GetUserAsync(keyValuePair.Key);
                var user = await _userRepository.GetOrCreateUserAsync(keyValuePair.Key, dsGuild.Id);
                var clownsCount = await _emojiCounterService.GetCurrentUserCount(user.UserId, user.GuildId, Emojis.ClownEmote);
                var nickname = dsUser?.Nickname ?? dsUser?.Username ?? user.Nickname;
                embedMessage.AddField($"#{index++}. {nickname}", BeautifyHelper.GetUserInfo(user, clownsCount, keyValuePair.Value));
            }

            await message.CommandMessageReplyAsync(null, embedMessage.Build(), TimeSpan.MaxValue);
        }

    }
}