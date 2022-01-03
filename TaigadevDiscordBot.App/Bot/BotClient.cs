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
using TaigadevDiscordBot.Core.Initialization;

namespace TaigadevDiscordBot.App.Bot
{
    public class BotClient : IHostedService, IDisposable
    {
        private readonly IBotConfiguration _configuration;
        private readonly DiscordSocketClient _botClient;
        private readonly IEnumerable<IInitializationModule> _initializationModules;
        private readonly ILogger<BotClient> _logger;

        public BotClient(
            IBotConfiguration configuration, 
            DiscordSocketClient botClient, 
            IUserEventHandler eventHandler, 
            IEnumerable<IInitializationModule> initializationModules, 
            ILogger<BotClient> logger)
        {
            _configuration = configuration;
            _botClient = botClient;
            _initializationModules = initializationModules;
            _logger = logger;
            // todo: store user roles too and give them back as he re-joins the server
            // todo: set initial role to newly joined users
            // events
            _botClient.UserVoiceStateUpdated += eventHandler.OnUserVoiceStateUpdated;
            _botClient.MessageReceived += eventHandler.OnMessageReceived;
            _botClient.ReactionAdded += eventHandler.OnReactionAdded;
            _botClient.Connected += BotClientOnConnected;
            _botClient.Ready += BotClientOnReady;
        }

        private async Task BotClientOnReady()
        {
            foreach (var initializationModule in _initializationModules)
            {
                await initializationModule.InitializeAsync(_botClient);
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _botClient.LoginAsync(TokenType.Bot, _configuration.Token);
                await _botClient.StartAsync();
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Bot cant start: {ex}");
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _botClient.StopAsync();
        }

        private Task BotClientOnConnected()
        {
            _logger.LogInformation($"Bot started");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _botClient.Dispose();
        }
    }
}