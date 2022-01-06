using System.Threading.Tasks;

using Discord.WebSocket;

namespace TaigadevDiscordBot.Core.Bot.Features.UserExperience
{
    public interface IUserLevelService
    {
        Task LevelUpUserIfNeededAsync(ulong userId, ulong guildId);
        
        Task<User> LevelUpUserAsync(ulong userId, ulong guildId);

        Task<User> SetUserLevelAsync(SocketGuildUser dsUser, int level);
    }
}