using System;

namespace TaigadevDiscordBot.Core.Bot.Features.UserActivity
{
    public class UserVoiceActivity
    {
        public UserVoiceActivity(ulong userId, ulong guildId, string username)
        {
            UserId = userId;
            GuildId = guildId;
            Username = username;
            VoiceEnterDateTime = DateTime.UtcNow;
            TimeInVoiceSpent = TimeSpan.Zero;
        }
        
        public DateTime VoiceEnterDateTime;
        public TimeSpan TimeInVoiceSpent;
        public ulong GuildId;
        public ulong UserId;
        public string Username;
    }
}