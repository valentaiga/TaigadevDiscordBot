using Discord.WebSocket;

namespace TaigadevDiscordBot.Core.Bot.Event.EventArgs
{
    public class NewTextMessageEventArgs
    {
        public NewTextMessageEventArgs(SocketMessage message, SocketUser user, SocketGuild guild)
        {
            Message = message;
            User = user;
            Guild = guild;
        }
        
        public SocketMessage Message;
        public SocketUser User;
        public SocketGuild Guild;
    }
}