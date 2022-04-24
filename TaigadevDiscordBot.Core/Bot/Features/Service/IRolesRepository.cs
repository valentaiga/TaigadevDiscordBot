using System.Collections.Generic;
using TaigadevDiscordBot.Core.Bot.Features.UserExperience;

namespace TaigadevDiscordBot.Core.Bot.Features.Service
{
    public interface IRolesRepository
    {
        
        CachedRole[] GetRoles(ulong guildId, IReadOnlyCollection<ulong> roleIds);
        void RemoveCachedRoles(ulong guildId);
    }
}