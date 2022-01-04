using System;
using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

using TaigadevDiscordBot.Core.Bot;
using TaigadevDiscordBot.Core.Bot.Features;
using TaigadevDiscordBot.Core.Extensions;

namespace TaigadevDiscordBot.App.Bot.Features.Commands.Profile
{
    public class GetProfileCommand : CommandBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IBotConfiguration _botConfiguration;

        public GetProfileCommand(IUserRepository userRepository, IBotConfiguration botConfiguration)
            : base("profile", "Get profile", $"t!profile @mention", false)
        {
            _userRepository = userRepository;
            _botConfiguration = botConfiguration;
        }

        public override async Task ExecuteAsync(SocketMessage message, SocketGuild guild)
        {
            var dsUser = message.MentionedUsers.Count == 1
                ? message.MentionedUsers.First() as SocketGuildUser
                : message.Author as SocketGuildUser;
            var user = await _userRepository.GetOrCreateUserAsync(dsUser.Id, guild.Id);

            var embedBuilder = new EmbedBuilder()
                .WithTitle($"{dsUser.Nickname} profile")
                .AddField("Level", user.Level, true)
                .AddField("Experience", user.Experience, true)
                .AddField("Cookies", user.CookiesCollected, true)
                .AddField("Total voice time", GetFormattedVoiceTime())
                .AddField("On server since", dsUser.JoinedAt!.Value.Date.ToString("Y"))
                .WithThumbnailUrl(dsUser.GetAvatarUrl(size: 80))
                .WithFooter($"Powered by TaigaBot", _botConfiguration.SelfUser.GetAvatarUrl(size: 16))
                .WithColor(Color.DarkRed);

            await message.Channel.SendAndRemoveMessageAsync(null, TimeSpan.MaxValue, embedBuilder.Build());
            
            string GetFormattedVoiceTime() => $"{user.TotalVoiceActivity.Days} days, {user.TotalVoiceActivity.Hours} hours, {user.TotalVoiceActivity.Minutes} minutes";
        }
    }
}