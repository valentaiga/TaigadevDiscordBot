using Discord.WebSocket;

namespace TaigadevDiscordBot.Core.Bot.Features.UserExperience
{
    public class CachedRole
    {
        public ulong Id { get; set; }
        public bool IsLevelRole { get; set; }
        public string Mention { get; set; }
        public string Name { get; set; }

        public int? Level => IsLevelRole ? int.Parse(Name.Split(".")[0]) : null;

        public static CachedRole From(SocketRole role, bool isLevelRole)
            => new CachedRole
            {
                Id = role.Id,
                IsLevelRole = isLevelRole,
                Mention = role.Mention,
                Name = role.Name
            };
    }
}