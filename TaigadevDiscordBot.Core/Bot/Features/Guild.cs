﻿using System.Collections.Generic;
using System.Text.Json.Serialization;

using TaigadevDiscordBot.Core.Database.Redis;

namespace TaigadevDiscordBot.Core.Bot.Features
{
    public class Guild : IRedisEntity
    {
        [JsonConstructor]
        public Guild()
        {
        }

        public Guild(ulong guildId)
        {
        }

        public ulong GuildId { get; set; }

        public IList<ulong> DefaultRoles { get; set; }

        public IList<ulong> IgnoredChannels { get; set; }

        public string GetCacheKey()
            => GetCacheKey(GuildId);

        public static string GetCacheKey(ulong guildId)
            => $"{guildId}:guildSettings";
    }
}