using System;
using System.Threading.Tasks;

using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using TaigadevDiscordBot.Core.Bot.Event;
using TaigadevDiscordBot.Core.Bot.Event.EventArgs;
using TaigadevDiscordBot.Core.Bot.Features.Commands;
using TaigadevDiscordBot.Core.Bot.Features.UserActivity;
using TaigadevDiscordBot.Core.Extensions;

namespace TaigadevDiscordBot.App.Bot
{
    public class UserEventHandler : IUserEventHandler
    {
        private readonly ILogger<UserEventHandler> _logger;

        private event Func<VoiceStatusUpdatedEventArgs, ValueTask> VoiceStatusUpdatedHandler;
        private event Func<NewTextMessageEventArgs, ValueTask> NewTextMessageHandler;

        public UserEventHandler(
            IVoiceActivityService voiceActivityService, 
            ITextActivityService textActivityService, 
            ICommandService commandService,
            ILogger<UserEventHandler> logger)
        {
            _logger = logger;
            VoiceStatusUpdatedHandler += voiceActivityService.UpdateUserVoiceActivityAsync;
            NewTextMessageHandler += textActivityService.UpdateUserTextActivityAsync;
            NewTextMessageHandler += commandService.ExecuteCommandAsync;
        }

        public async Task OnUserVoiceStateUpdated(SocketUser user, SocketVoiceState oldVoiceState, SocketVoiceState newVoiceState)
        {
            if (VoiceStatusUpdatedHandler is not null && !user.IsBot)
            {
                var eventArgs = new VoiceStatusUpdatedEventArgs(user, GetGuild(), GetUserVoiceStatus());
                try
                {
                    await VoiceStatusUpdatedHandler(eventArgs);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error during message processing: {ex}");
                }
            }

            UserVoiceStatus GetUserVoiceStatus()
                => newVoiceState.IsNull() ? UserVoiceStatus.Left
                    : newVoiceState.IsUserMuted() ? UserVoiceStatus.Muted
                    : UserVoiceStatus.Unmuted;

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
    }
}