using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

using TaigadevDiscordBot.Core.Bot;
using TaigadevDiscordBot.Core.Bot.Features.Commands;
using TaigadevDiscordBot.Core.Bot.Features.Service;
using TaigadevDiscordBot.Core.Extensions;

namespace TaigadevDiscordBot.App.Bot.Features.Commands.Unspecified
{
    public class UnignoreChannelCommand : CommandBase
    {
        private readonly IGuildRepository _guildRepository;

        public UnignoreChannelCommand(IBotConfiguration botConfiguration, IGuildRepository guildRepository)
            : base(
                "unignore",
                "Allow the use of commands in the text channel",
                $"{botConfiguration.Prefix}unignore",
                true,
                GuildPermission.Administrator,
                allowToUseInIgnoredChannel: true)
        {
            _guildRepository = guildRepository;
        }

        public override async Task ExecuteAsync(SocketMessage message, IGuild dsGuild)
        {
            var guild = await _guildRepository.GetGuildAsync(dsGuild.Id);
            var textChannelId = message.Channel.Id;
            if (guild.IgnoredChannels.Contains(textChannelId))
            {
                guild.IgnoredChannels.Remove(textChannelId);
                await _guildRepository.SaveGuildAsync(guild);
                await message.CommandMessageReplyAsync(
                    $"Text channel is unignored. Commands are available in '{message.Channel.Name}'");
            }
            else
            {
                await message.CommandMessageReplyAsync($"This channel is not ignored. There is no need in '{Command}'");
            }
        }
    }
}