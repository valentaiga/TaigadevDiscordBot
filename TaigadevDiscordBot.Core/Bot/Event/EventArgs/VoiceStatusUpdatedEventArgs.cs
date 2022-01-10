using Discord;
using Discord.WebSocket;

namespace TaigadevDiscordBot.Core.Bot.Event.EventArgs
{
    public class VoiceStatusUpdatedEventArgs
    {
        public VoiceStatusUpdatedEventArgs(SocketGuildUser user, IGuild guild, SocketVoiceChannel previousChannel, SocketVoiceChannel currentChannel)
        {
            User = user;
            Guild = guild;
            PreviousChannel = previousChannel;
            CurrentChannel = currentChannel;
        }

        public SocketGuildUser User;
        public IGuild Guild;
        public SocketVoiceChannel PreviousChannel;
        public SocketVoiceChannel CurrentChannel;
    }
}