using System;

using Microsoft.Extensions.Configuration;

using TaigadevDiscordBot.App.Constants;
using TaigadevDiscordBot.Core.Bot;

namespace TaigadevDiscordBot.App.Bot
{
    public class BotConfiguration : IBotConfiguration
    {
        public BotConfiguration(IConfiguration configuration)
        {
            AdminId = configuration[ConfigurationKeys.Discord.AdminId];
            Token = Environment.GetEnvironmentVariable(ConfigurationKeys.Discord.Token);
        }

        public string AdminId { get; }
        public string Token { get; }
    }
}