using System;

namespace TaigadevDiscordBot.Core.Bot.Features.UserExperience
{
    public interface IExperienceCalculationService
    {
        ulong CalculateVoiceExperience(TimeSpan voiceTime);

        ulong CalculateChatMessageExperience(ulong messagesCount);
    }
}