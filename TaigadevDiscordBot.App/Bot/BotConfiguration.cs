using System;
using System.Collections.Generic;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using TaigadevDiscordBot.Core.Bot;
using TaigadevDiscordBot.Core.Bot.Features.Service;
using TaigadevDiscordBot.Core.Constants;

namespace TaigadevDiscordBot.App.Bot
{
    public class BotConfiguration : IBotConfiguration
    {
        public BotConfiguration(IConfiguration configuration)
        {
            AdminId = configuration[ConfigurationKeys.Discord.AdminId];
            Token = Environment.GetEnvironmentVariable(ConfigurationKeys.Discord.Token);
            Prefix = configuration[ConfigurationKeys.Discord.Prefix];
            ServiceCategoryName = configuration[ConfigurationKeys.Discord.ServiceCategoryName];
            WorkOnServerIds = configuration.GetSection(ConfigurationKeys.Discord.WorkServerIds).Get<List<ulong>>();
            ServiceChannels = configuration.GetSection(ConfigurationKeys.Discord.ServiceChannels).Get<List<GuildChannel>>();
        }

        public void SetSelfUser(SocketSelfUser selfUser) => SelfUser = selfUser;

        public string AdminId { get; }
        public string Token { get; }
        public string Prefix { get; }

        public IList<ulong> WorkOnServerIds { get; }
        public string ServiceCategoryName { get; }
        public List<GuildChannel> ServiceChannels { get; }
        public SocketSelfUser SelfUser { get; private set; }
    }
}