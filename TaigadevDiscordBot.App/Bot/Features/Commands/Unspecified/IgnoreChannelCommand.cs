using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

using TaigadevDiscordBot.Core.Bot;
using TaigadevDiscordBot.Core.Bot.Features.Commands;
using TaigadevDiscordBot.Core.Bot.Features.Service;
using TaigadevDiscordBot.Core.Extensions;

namespace TaigadevDiscordBot.App.Bot.Features.Commands.Unspecified
{
    public class IgnoreChannelCommand : CommandBase
    {
        private readonly IGuildRepository _guildRepository;

        public IgnoreChannelCommand(IBotConfiguration botConfiguration, IGuildRepository guildRepository) 
            : base(
                "ignore", 
                "Prohibit the use of commands in the text channel", 
                $"{botConfiguration.Prefix}ignore", 
                true, 
                GuildPermission.Administrator)
        {
            _guildRepository = guildRepository;
        }

        public override async Task ExecuteAsync(SocketMessage message, SocketGuild dsGuild)
        {
            var guild = await _guildRepository.GetGuildAsync(dsGuild.Id);
            var textChannelId = message.Channel.Id;
            if (!guild.IgnoredChannels.Contains(textChannelId))
            {
                guild.IgnoredChannels.Add(textChannelId);
                await _guildRepository.SaveGuildAsync(guild);
                await message.CommandMessageReplyAsync(
                    $"Text channel is ignored. No commands can be executed in '{message.Channel.Name}'.");
            }
        }
    }
}