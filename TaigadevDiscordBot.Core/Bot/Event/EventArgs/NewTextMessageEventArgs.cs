using Discord.WebSocket;

namespace TaigadevDiscordBot.Core.Bot.Event.EventArgs
{
    public class NewTextMessageEventArgs
    {
        public NewTextMessageEventArgs(SocketMessage message, SocketGuildUser user, SocketGuild guild)
        {
            Message = message;
            User = user;
            Guild = guild;
        }
        
        public SocketMessage Message;
        public SocketGuildUser User;
        public SocketGuild Guild;
    }
}