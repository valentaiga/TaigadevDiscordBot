using System;
using System.Threading.Tasks;

namespace TaigadevDiscordBot.Core.Bot.Features
{
    public interface IUserRepository
    {
        Task<User> GetOrCreateUserAsync(ulong userId, ulong guildId);

        Task SaveUserAsync(User user);

        Task<User> UpdateUserAsync(ulong userId, ulong guildId, Func<User, Task> updateAction);
    }
}