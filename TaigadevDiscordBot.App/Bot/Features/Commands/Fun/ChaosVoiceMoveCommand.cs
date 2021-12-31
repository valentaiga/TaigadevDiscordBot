using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

using Discord.WebSocket;

using TaigadevDiscordBot.Core.Bot.Features.Commands;
using TaigadevDiscordBot.Core.Extensions;

namespace TaigadevDiscordBot.App.Bot.Features.Commands.Fun
{
    public class ChaosVoiceMoveCommand : ITextChannelCommand
    {
        private readonly Random _random;

        public ChaosVoiceMoveCommand()
        {
            Command = "chaosmove";
            _random = new();
        }

        public string Command { get; }

        public async Task ExecuteAsync(SocketMessage message, SocketGuild guild)
        {
            if (message.MentionedUsers.FirstOrDefault() is not SocketGuildUser mentionedUser)
            {
                await message.CommandMessageReplyAsync($"Command '{Command}' requires user to mention.");
                return;
            }

            var currentUserVoiceChannel = guild.VoiceChannels.FirstOrDefault(x => x.Users.Any(x => x.Id == mentionedUser.Id));

            if (currentUserVoiceChannel is null)
            {
                await message.CommandMessageReplyAsync($"Command '{Command}' requires mentioned user to be in voice channel.");
                return;
            }

            var availableChannelsToMove = guild.VoiceChannels.ToImmutableArray();

            if (availableChannelsToMove.Length == 0)
            {
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

            ulong GetRandomChannel() => availableChannelsToMove[_random.Next(0, availableChannelsToMove.Length)].Id;
        }
    }
}