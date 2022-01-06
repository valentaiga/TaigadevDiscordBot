using TaigadevDiscordBot.Core.Bot.Features;
using TaigadevDiscordBot.Core.Constants;

namespace TaigadevDiscordBot.Core.Helpers
{
    public static class BeautifyHelper
    {
        public static string GetUserInfo(User user, int clowns, int cookies)
            => $"**Level:** {user.Level} | **Experience**: {user.Experience} | {Emojis.ClownEmote} {clowns} | {Emojis.CookieEmote} {cookies}";
    }
}