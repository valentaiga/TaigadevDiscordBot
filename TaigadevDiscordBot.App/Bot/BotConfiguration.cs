using System;
using System.Collections.Generic;

using Microsoft.Extensions.Configuration;

using TaigadevDiscordBot.App.Constants;
using TaigadevDiscordBot.Core.Bot;
using TaigadevDiscordBot.Core.Bot.Features.Service;

namespace TaigadevDiscordBot.App.Bot
{
    public class BotConfiguration : IBotConfiguration
    {
        public BotConfiguration(IConfiguration configuration)
        {
            AdminId = configuration[ConfigurationKeys.Discord.AdminId];
            Token = Environment.GetEnvironmentVariable(ConfigurationKeys.Discord.Token);
            ServiceCategoryName = configuration[ConfigurationKeys.Discord.ServiceCategoryName];
            WorkOnServerIds = configuration.GetSection(ConfigurationKeys.Discord.WorkServerIds).Get<List<ulong>>();
            ServiceChannels = configuration.GetSection(ConfigurationKeys.Discord.ServiceChannels).Get<List<GuildChannel>>();
        }

        public string AdminId { get; }
        public string Token { get; }
        
        public IList<ulong> WorkOnServerIds { get; }
        public string ServiceCategoryName { get; }
        public IList<GuildChannel> ServiceChannels { get; }
    }
}