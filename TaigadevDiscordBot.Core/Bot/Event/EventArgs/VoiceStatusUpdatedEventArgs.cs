using Discord.WebSocket;

namespace TaigadevDiscordBot.Core.Bot.Event.EventArgs
{
    public struct VoiceStatusUpdatedEventArgs
    {
        public VoiceStatusUpdatedEventArgs(SocketUser user, SocketGuild guild, UserVoiceStatus voiceStatus)
        {
            User = user;
            Guild = guild;
            VoiceStatus = voiceStatus;
        }

        public SocketUser User;
        public SocketGuild Guild;
        public UserVoiceStatus VoiceStatus;
    }
}