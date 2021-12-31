using Discord.WebSocket;

namespace TaigadevDiscordBot.Core.Extensions
{
    public static class VoiceChannelExtensions
    {
        public static bool IsNull(this SocketVoiceState socketVoiceState)
            => socketVoiceState.VoiceChannel is null;

        public static bool IsUserMuted(this SocketVoiceState socketVoiceState)
            => socketVoiceState.IsSelfMuted || socketVoiceState.IsMuted || socketVoiceState.IsDeafened;
    }
}