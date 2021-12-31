using System;

using Microsoft.Extensions.Configuration;

using TaigadevDiscordBot.App.Constants;
using TaigadevDiscordBot.Core.Database.Redis;

namespace TaigadevDiscordBot.App.Database.Redis
{
    public class RedisConfiguration : IRedisConfiguration
    {
        public RedisConfiguration(IConfiguration configuration)
        {
            Endpoint = Environment.GetEnvironmentVariable(ConfigurationKeys.Redis.Endpoint);
            Password = Environment.GetEnvironmentVariable(ConfigurationKeys.Redis.Password);
            Prefix = configuration[ConfigurationKeys.Redis.Prefix];
        }

        public string Endpoint { get; }

        public string Password { get; }

        public string Prefix { get; set; }
    }
}