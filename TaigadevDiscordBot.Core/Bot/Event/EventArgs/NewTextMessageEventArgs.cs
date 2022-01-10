using Discord;
using Discord.WebSocket;

namespace TaigadevDiscordBot.Core.Bot.Event.EventArgs
{
    public class NewTextMessageEventArgs
    {
        public NewTextMessageEventArgs(SocketMessage message, IGuildUser user, IGuild guild)
        {
            Message = message;
            User = user;
            Guild = guild;
        }
        
        public SocketMessage Message;
        public IGuildUser User;
        public IGuild Guild;
    }
}