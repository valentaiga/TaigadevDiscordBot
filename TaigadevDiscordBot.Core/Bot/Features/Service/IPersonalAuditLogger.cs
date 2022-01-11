using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaigadevDiscordBot.Core.Bot.Features.Service
{
    public interface IPersonalAuditLogger
    {
        public Task AuditAsync(ulong userId, ulong guildId, string message);

        public Task<IEnumerable<string>> GetPersonalLogsAsync(ulong userId, ulong guildId);

        Task TrackUserAsync(ulong userId, ulong guildId);
    }
}