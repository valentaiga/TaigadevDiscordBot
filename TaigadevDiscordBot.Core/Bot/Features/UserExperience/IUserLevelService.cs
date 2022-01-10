using System.Threading.Tasks;

using Discord;

namespace TaigadevDiscordBot.Core.Bot.Features.UserExperience
{
    public interface IUserLevelService
    {
        Task LevelUpUserIfNeededAsync(ulong userId, ulong guildId);
        
        Task<User> LevelUpUserAsync(ulong userId, ulong guildId);

        Task<User> SetUserLevelAsync(IGuildUser dsUser, int level);
    }
}