using System;
using System.Threading;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

using Microsoft.Extensions.Hosting;

using TaigadevDiscordBot.Core.Bot;
using TaigadevDiscordBot.Core.Bot.Event;

namespace TaigadevDiscordBot.App.Bot
{
    public class BotClient : IHostedService, IDisposable
    {
        private readonly IBotConfiguration _configuration;
        private readonly DiscordSocketClient _botClient;

        public BotClient(IBotConfiguration configuration, DiscordSocketClient botClient, IUserEventHandler eventHandler)
        {
            _configuration = configuration;
            _botClient = botClient;
            
            // events
            _botClient.UserVoiceStateUpdated += eventHandler.OnUserVoiceStateUpdated;
            _botClient.MessageReceived += eventHandler.OnMessageReceived;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _botClient.LoginAsync(TokenType.Bot, _configuration.Token);
            await _botClient.StartAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _botClient.StopAsync();
        }

        public void Dispose()
        {
            _botClient.Dispose();
        }
    }
}