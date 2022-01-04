using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaigadevDiscordBot.Core.Bot.Features.Service
{
    public interface IAuditLogger
    {
        Task LogErrorAsync(Exception exception, ulong guildId);

        Task LogInformationAsync(string message, ulong guildId, IDictionary<string, string> embedMessageFields = null);
    }
}