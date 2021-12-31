using System;
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

        public string Username { get; set; }

        public ulong Experience { get; set; }

        public int Level { get; set; }

        // todo: json serialize settings for timespan (use just string instead of json structure)
        [JsonIgnore]
        public TimeSpan TotalVoiceActivity { get; set; }

        [JsonPropertyName("TotalVoiceActivity")]
        public string TimeInVoiceSpentString
        {
            get => TotalVoiceActivity.ToString();
            set => TotalVoiceActivity = TimeSpan.TryParse(value, out var result) ? result : default;
        }

        public string GetCacheKey()
            => GetCacheKey(GuildId, UserId);

        public static string GetCacheKey(ulong guildId, ulong userId) 
            => $"{guildId}:users:{userId}";
    }
}