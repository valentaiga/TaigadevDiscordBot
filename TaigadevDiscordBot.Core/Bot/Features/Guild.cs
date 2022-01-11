using System.Collections.Generic;
using System.Text.Json.Serialization;

using TaigadevDiscordBot.Core.Database.Redis;

namespace TaigadevDiscordBot.Core.Bot.Features
{
    public class Guild : IRedisEntity
    {
        [JsonConstructor]
        public Guild()
        {
            DefaultRoles = new List<ulong>();
            IgnoredChannels = new List<ulong>();
            ProhibitExperienceUsers = new List<ulong>();
        }

        public Guild(ulong guildId) : this()
        {
            GuildId = guildId;
        }

        public ulong GuildId { get; set; }

        public IList<ulong> DefaultRoles { get; set; }

        public IList<ulong> IgnoredChannels { get; set; }

        public IList<ulong> ProhibitExperienceUsers { get; set; }

        public string GetCacheKey()
            => GetCacheKey(GuildId);

        public static string GetCacheKey(ulong guildId)
            => $"{guildId}:guildSettings";
    }
}