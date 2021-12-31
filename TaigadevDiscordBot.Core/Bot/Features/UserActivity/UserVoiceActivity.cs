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
            LastActivityDateTime = DateTime.UtcNow;
            TimeInVoiceSpent = TimeSpan.Zero;
        }

        public ulong UserId;
        public ulong GuildId;
        public string Username;
        public DateTime LastActivityDateTime;
        public TimeSpan TimeInVoiceSpent;
    }
}