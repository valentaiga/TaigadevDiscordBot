using Discord;
using Discord.WebSocket;

namespace TaigadevDiscordBot.Core.Bot.Event.EventArgs
{
    public class ReactionAddedEventArgs
    {
        public ReactionAddedEventArgs(IUserMessage message, SocketTextChannel textChannel, SocketReaction reaction)
        {
            Message = message;
            TextChannel = textChannel;
            Reaction = reaction;
        }

        public IUserMessage Message;
        public SocketTextChannel TextChannel;
        public SocketReaction Reaction;
    }
}