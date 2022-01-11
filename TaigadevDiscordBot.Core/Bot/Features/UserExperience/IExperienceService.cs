using System;
using System.Threading.Tasks;

namespace TaigadevDiscordBot.Core.Bot.Features.UserExperience
{
    public interface IExperienceService
    {
        Task<ulong> CalculateVoiceExperienceAsync(ulong userId, ulong guildId, TimeSpan voiceTime);

        Task<ulong> CalculateMessageExperienceAsync(ulong userId, ulong guildId);

        Task ProhibitExperienceGainAsync(ulong userId, ulong guildId);

        Task AllowExperienceGainAsync(ulong userId, ulong guildId);
    }
}