using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TaigadevDiscordBot.Core.Bot;
using TaigadevDiscordBot.Core.Bot.Event;
using TaigadevDiscordBot.Core.Bot.Features.Service;
using TaigadevDiscordBot.Core.Initialization;

namespace TaigadevDiscordBot.App.Bot
{
    public class BotClient : IHostedService, IDisposable
    {
        private readonly IBotConfiguration _botConfiguration;
        private readonly DiscordSocketClient _botClient;
        private readonly IAuditLogger _auditLogger;
        private readonly IEnumerable<IInitializationModule> _initializationModules;
        private readonly ILogger<BotClient> _logger;

        public BotClient(
            IBotConfiguration botConfiguration, 
            DiscordSocketClient botClient, 
            IUserEventHandler eventHandler,
            IAuditLogger auditLogger,
            IEnumerable<IInitializationModule> initializationModules, 
            ILogger<BotClient> logger)
        {
            _botConfiguration = botConfiguration;
            _botClient = botClient;
            _auditLogger = auditLogger;
            _initializationModules = initializationModules;
            _logger = logger;
            // todo: store user roles too and give them back as he re-joins the server
            // todo: set initial role to newly joined users
            // events
            _botClient.UserVoiceStateUpdated += eventHandler.OnUserVoiceStateUpdated;
            _botClient.MessageReceived += eventHandler.OnMessageReceived;
            _botClient.ReactionAdded += eventHandler.OnReactionAdded;
            _botClient.UserJoined += eventHandler.OnUserJoined;
            _botClient.Connected += BotClientOnConnected;
            _botClient.Ready += BotClientOnReady;
        }

        private async Task BotClientOnReady()
        {
            _botConfiguration.SetSelfUser(_botClient.CurrentUser);
            foreach (var initializationModule in _initializationModules)
            {
                await initializationModule.InitializeAsync(_botClient);
            }

            foreach (var guild in _botClient.Guilds)
            {
                await _auditLogger.LogInformationAsync("Bot connected", guild.Id);
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _botClient.LoginAsync(TokenType.Bot, _botConfiguration.Token);
                await _botClient.StartAsync();
                _logger.LogInformation("Bot started");
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Bot cant start: {ex}");
                throw;
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var guild in _botClient.Guilds)
            {
                await _auditLogger.LogInformationAsync("Bot is shutting down", guild.Id);
            }
            await _botClient.StopAsync();
        }

        private Task BotClientOnConnected()
        {
            _logger.LogInformation($"Bot connected");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _botClient.Dispose();
        }
    }
}