using System.Threading.Tasks;

namespace TaigadevDiscordBot.Core.Bot.Features.UserExperience
{
    public interface IUserLevelService
    {
        Task LevelUpUserIfNeeded(ulong userId, ulong guildId);

        Task LevelUpUserAsync(ulong userId, ulong guildId);
    }
}