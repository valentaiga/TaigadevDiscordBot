using Discord.WebSocket;

namespace TaigadevDiscordBot.Core.Extensions
{
    public static class GuildUserExtensions
    {
        public static bool IsMuted(this SocketGuildUser user)
            => user.IsMuted || user.IsSelfMuted || user.IsDeafened || user.IsSelfDeafened;
    }
}