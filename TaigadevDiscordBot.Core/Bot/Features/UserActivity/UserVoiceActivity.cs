using System;
using System.Collections.Generic;
using System.Linq;

using Discord.WebSocket;

namespace TaigadevDiscordBot.Core.Bot.Features.UserActivity
{
    public class UserVoiceActivity
    {
        public UserVoiceActivity(ulong userId, ulong guildId, string username, IReadOnlyCollection<SocketRole> roles)
        {
            UserId = userId;
            GuildId = guildId;
            Username = username;
            VoiceEnterDateTime = DateTime.UtcNow;
            TimeInVoiceSpent = TimeSpan.Zero;
            Roles = roles.Where(x => !x.IsEveryone).Select(x => x.Id).ToList();
        }
        
        public DateTime VoiceEnterDateTime;
        public TimeSpan TimeInVoiceSpent;
        public ulong GuildId;
        public ulong UserId;
        public string Username;
        public IReadOnlyList<ulong> Roles;
    }
}