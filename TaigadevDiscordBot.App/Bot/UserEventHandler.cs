using System;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

using Microsoft.Extensions.Logging;

using TaigadevDiscordBot.Core.Bot.Event;
using TaigadevDiscordBot.Core.Bot.Event.EventArgs;
using TaigadevDiscordBot.Core.Bot.Features.Collectors;
using TaigadevDiscordBot.Core.Bot.Features.Commands;
using TaigadevDiscordBot.Core.Bot.Features.Service;
using TaigadevDiscordBot.Core.Bot.Features.UserActivity;

namespace TaigadevDiscordBot.App.Bot
{
    public class UserEventHandler : IUserEventHandler
    {
        private readonly IAuditLogger _auditLogger;
        private readonly ILogger<UserEventHandler> _logger;

        private event Func<VoiceStatusUpdatedEventArgs, ValueTask> VoiceStatusUpdatedHandler;
        private event Func<NewTextMessageEventArgs, ValueTask> NewTextMessageHandler;
        private event Func<ReactionAddedEventArgs, ValueTask> ReactionAddedHandler;
        private event Func<UserJoinedEventArgs, ValueTask> UserJoinedHandler;

        public UserEventHandler(
            IVoiceActivityService voiceActivityService, 
            ITextActivityService textActivityService,
            IEmojiCounterService emojiCounterService,
            ICommandService commandService,
            IRolesService rolesService,
            IAuditLogger auditLogger,
            ILogger<UserEventHandler> logger)
        {
            _auditLogger = auditLogger;
            _logger = logger;
            VoiceStatusUpdatedHandler += voiceActivityService.UpdateUserVoiceActivityAsync;
            NewTextMessageHandler += textActivityService.UpdateUserTextActivityAsync;
            NewTextMessageHandler += commandService.ExecuteCommandAsync;
            ReactionAddedHandler += emojiCounterService.IncrementUserEmojiCount;
            UserJoinedHandler += rolesService.SetRolesToNewUser;
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
                    await _auditLogger.LogErrorAsync(ex, guildUser.Guild.Id);
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
                var eventArgs = new NewTextMessageEventArgs(message, message.Author as SocketGuildUser, textChannel.Guild);
                try
                {
                    await NewTextMessageHandler(eventArgs);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error during message processing: {ex}");
                    await _auditLogger.LogErrorAsync(ex, textChannel.Guild.Id);
                }
            }
        }

        public async Task OnReactionAdded(Cacheable<IUserMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> cachedTextChannel, SocketReaction reaction)
        {
            var message = await cachedMessage.GetOrDownloadAsync();
            var textChannel = await cachedTextChannel.GetOrDownloadAsync() as SocketTextChannel;
            if (message is not null
                && textChannel is not null)
            {
                var eventArgs = new ReactionAddedEventArgs(message, textChannel, reaction);
                try
                {
                    await ReactionAddedHandler!(eventArgs);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error during message processing: {ex}");
                    await _auditLogger.LogErrorAsync(ex, textChannel.Guild.Id);
                }
            }
        }

        public async Task OnUserJoined(SocketGuildUser dsUser)
        {
            var eventArgs = new UserJoinedEventArgs(dsUser);
            try
            {
                await UserJoinedHandler!(eventArgs);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during message processing: {ex}");
                await _auditLogger.LogErrorAsync(ex, dsUser.Guild.Id);
            }
        }
    }
}