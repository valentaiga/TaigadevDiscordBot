using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

using TaigadevDiscordBot.Core.Database.Redis;

namespace TaigadevDiscordBot.Core.Bot.Features
{
    public class User : IRedisEntity
    {
        private User()
        {
            TotalVoiceActivity = TimeSpan.Zero;
        }

        public User(ulong guildId, ulong userId) : this()
        {
            GuildId = guildId;
            UserId = userId;
        }

        public ulong UserId { get; set; }
        
        public ulong GuildId { get; set; }

        public string Nickname { get; set; }

        public ulong Experience { get; set; }

        public int Level { get; set; }

        public int CookiesCollected { get; set; }

        public IReadOnlyList<ulong> Roles { get; set; } = new List<ulong>(0);

        [JsonIgnore]
        public TimeSpan TotalVoiceActivity { get; set; }

        [JsonPropertyName("TotalVoiceActivity")]
        public string TimeInVoiceSpentString
        {
            get => TotalVoiceActivity.ToString();
            set => TotalVoiceActivity = TimeSpan.TryParse(value, out var result) ? result : default;
        }

        public bool LevelMigrationNotNeeded { get; set; }

        public string GetCacheKey()
            => GetCacheKey(GuildId, UserId);

        public static string GetCacheKey(ulong guildId, ulong userId) 
            => $"{guildId}:users:{userId}";
    }
}