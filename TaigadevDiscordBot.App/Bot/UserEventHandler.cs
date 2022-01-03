using System;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

using Microsoft.Extensions.Logging;

using TaigadevDiscordBot.Core.Bot.Event;
using TaigadevDiscordBot.Core.Bot.Event.EventArgs;
using TaigadevDiscordBot.Core.Bot.Features.Commands;
using TaigadevDiscordBot.Core.Bot.Features.UserActivity;

namespace TaigadevDiscordBot.App.Bot
{
    public class UserEventHandler : IUserEventHandler
    {
        private readonly ITextActivityService _textActivityService;
        private readonly ILogger<UserEventHandler> _logger;

        private event Func<VoiceStatusUpdatedEventArgs, ValueTask> VoiceStatusUpdatedHandler;
        private event Func<NewTextMessageEventArgs, ValueTask> NewTextMessageHandler;

        public UserEventHandler(
            IVoiceActivityService voiceActivityService, 
            ITextActivityService textActivityService, 
            ICommandService commandService,
            ILogger<UserEventHandler> logger)
        {
            _textActivityService = textActivityService;
            _logger = logger;
            VoiceStatusUpdatedHandler += voiceActivityService.UpdateUserVoiceActivityAsync;
            NewTextMessageHandler += textActivityService.UpdateUserTextActivityAsync;
            NewTextMessageHandler += commandService.ExecuteCommandAsync;
        }

        public async Task OnUserVoiceStateUpdated(SocketUser user, SocketVoiceState oldVoiceState, SocketVoiceState newVoiceState)
        {
            if (VoiceStatusUpdatedHandler is not null && !user.IsBot && user is SocketGuildUser guildUser)
            {
                var eventArgs = new VoiceStatusUpdatedEventArgs(guildUser, GetGuild(), oldVoiceState.VoiceChannel, newVoiceState.VoiceChannel);
                try
                {
                    await VoiceStatusUpdatedHandler(eventArgs);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error during voice activity: {ex}");
                }
            }

            SocketGuild GetGuild()
                => oldVoiceState.VoiceChannel?.Guild ?? newVoiceState.VoiceChannel?.Guild;
        }

        public async Task OnMessageReceived(SocketMessage message)
        {
            if (NewTextMessageHandler is not null 
                && !message.Author.IsBot
                && message.Channel is SocketGuildChannel textChannel)
            {
                var eventArgs = new NewTextMessageEventArgs(message, message.Author, textChannel.Guild);
                try
                {
                    await NewTextMessageHandler(eventArgs);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error during message processing: {ex}");
                }
            }
        }

        public async Task OnReactionAdded(Cacheable<IUserMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> cachedTextChannel, SocketReaction reaction)
        {
            const string cookieEmote = @"🍪";
            var message = await cachedMessage.GetOrDownloadAsync();
            if (!message.Author.IsBot 
                && message.Author.Id != reaction.UserId
                && reaction.Emote.Name == cookieEmote)
            {
                var textChannel = await cachedTextChannel.GetOrDownloadAsync() as SocketTextChannel;
                await _textActivityService.IncrementUserCookiesAsync(message.Author.Id, textChannel!.Guild.Id);
            }
        }
    }
}