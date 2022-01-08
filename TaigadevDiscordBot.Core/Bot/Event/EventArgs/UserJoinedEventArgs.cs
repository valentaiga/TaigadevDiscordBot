using Discord.WebSocket;

namespace TaigadevDiscordBot.Core.Bot.Event.EventArgs
{
    public class UserJoinedEventArgs
    {
        public UserJoinedEventArgs(SocketGuildUser user)
        {
            User = user;
        }

        public SocketGuildUser User;
    }
}