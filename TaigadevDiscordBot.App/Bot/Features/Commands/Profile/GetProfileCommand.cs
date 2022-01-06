using System;
using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

using TaigadevDiscordBot.Core.Bot;
using TaigadevDiscordBot.Core.Bot.Features;
using TaigadevDiscordBot.Core.Bot.Features.Commands;
using TaigadevDiscordBot.Core.Bot.Features.UserActivity;
using TaigadevDiscordBot.Core.Extensions;

namespace TaigadevDiscordBot.App.Bot.Features.Commands.Profile
{
    public class GetProfileCommand : CommandBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IClownCollectorService _clownCollectorService;
        private readonly IBotConfiguration _botConfiguration;

        public GetProfileCommand(
            IUserRepository userRepository, 
            IClownCollectorService clownCollectorService, 
            IBotConfiguration botConfiguration)
            : base(
                "profile", 
                "Get profile", 
                $"t!profile @mention", 
                false,
                GuildPermission.SendMessages)
        {
            _userRepository = userRepository;
            _clownCollectorService = clownCollectorService;
            _botConfiguration = botConfiguration;
        }

        public override async Task ExecuteAsync(SocketMessage message, SocketGuild guild)
        {
            var dsUser = message.MentionedUsers.Count == 1
                ? message.MentionedUsers.First() as SocketGuildUser
                : message.Author as SocketGuildUser;
            var user = await _userRepository.GetOrCreateUserAsync(dsUser!.Id, guild.Id);
            var clownsCollectedCount = await _clownCollectorService.GetCurrentUserCount(dsUser.Id, guild.Id);

            var embedBuilder = new EmbedBuilder()
                .WithTitle($"{dsUser.Nickname ?? dsUser.Username} profile")
                .AddField("Level", user.Level, true)
                .AddField("Experience", user.Experience, true)
                .AddField("Clowns", clownsCollectedCount, false)
                .AddField("Total voice time", GetFormattedVoiceTime())
                .AddField("On server since", dsUser.JoinedAt!.Value.Date.ToString("Y"))
                .WithThumbnailUrl(dsUser.GetAvatarUrl(size: 80))
                .AdjustBotFields(_botConfiguration, Color.DarkRed);

            await message.Channel.SendAndRemoveMessageAsync(null, TimeSpan.MaxValue, embedBuilder.Build());

            string GetFormattedVoiceTime() => $"{user.TotalVoiceActivity.Days} days, {user.TotalVoiceActivity.Hours} hours, {user.TotalVoiceActivity.Minutes} minutes";
        }
    }
}