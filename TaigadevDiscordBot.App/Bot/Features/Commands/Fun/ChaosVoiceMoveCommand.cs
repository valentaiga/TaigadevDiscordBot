using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

using TaigadevDiscordBot.Core.Bot;
using TaigadevDiscordBot.Core.Bot.Features.Commands;
using TaigadevDiscordBot.Core.Extensions;

namespace TaigadevDiscordBot.App.Bot.Features.Commands.Fun
{
    public class ChaosVoiceMoveCommand : CommandBase
    {
        private readonly Random _random = new();

        public ChaosVoiceMoveCommand(IBotConfiguration botConfiguration)
            : base(
                "chaosmove", 
                "Chaotic user movement between voice channels", 
                $"{botConfiguration.Prefix}chaosmove @mention", 
                true,
                GuildPermission.MoveMembers)
        {
        }

        public override async Task ExecuteAsync(SocketMessage message, IGuild dsGuild)
        {
            if (message.MentionedUsers.FirstOrDefault() is not SocketGuildUser mentionedUser)
            {
                await message.CommandMessageReplyAsync($"Command '{Command}' requires user to mention.");
                return;
            }

            if (mentionedUser.VoiceChannel is null)
            {
                await message.CommandMessageReplyAsync($"Command '{Command}' requires mentioned user to be in voice channel.");
                return;
            }

            var voiceChannels = (await dsGuild.GetVoiceChannelsAsync()).ToImmutableArray();
            var currentUserVoiceChannel = voiceChannels.FirstOrDefault(x => x.Id == mentionedUser.VoiceChannel.Id);

            if (currentUserVoiceChannel is null)
            {
                await message.CommandMessageReplyAsync($"Command '{Command}' requires mentioned user to be in voice channel.");
                return;
            }

            if (voiceChannels.Length < 3)
            {
                await message.CommandMessageReplyAsync($"Command '{Command}' requires at least 3 voice channels.");
                return;
            }

            const int totalMoves = 9;
            try
            {
                var previousChannelId = currentUserVoiceChannel.Id;
                for (var i = 0; i < totalMoves; i++)
                {
                    var nextVoiceChannelId = GenerateChannelToMove(previousChannelId);
                    await mentionedUser.ModifyAsync(x => x.ChannelId = nextVoiceChannelId);
                    previousChannelId = nextVoiceChannelId;
                    await Task.Delay(TimeSpan.FromSeconds(0.5));
                }

                await mentionedUser.ModifyAsync(x => x.ChannelId = currentUserVoiceChannel.Id);
            }
            catch {}

            ulong GenerateChannelToMove(ulong previousChannelId)
            {
                var nextVoiceChannelId = GetRandomChannel();
                for (var i = 0; i < 5 && previousChannelId == nextVoiceChannelId; i++)
                {
                    nextVoiceChannelId = GetRandomChannel();
                }

                return nextVoiceChannelId;
            }

            ulong GetRandomChannel() => voiceChannels[_random.Next(0, voiceChannels.Length)].Id;
        }
    }
}