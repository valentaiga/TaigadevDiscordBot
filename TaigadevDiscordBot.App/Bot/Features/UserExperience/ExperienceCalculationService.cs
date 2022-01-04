using System;
using System.Linq;

using TaigadevDiscordBot.Core.Bot.Features.UserExperience;

namespace TaigadevDiscordBot.App.Bot.Features.UserExperience
{
#pragma warning disable 162
    public class ExperienceCalculationService : IExperienceCalculationService
    {
        private readonly Random _random = new();

        public ulong CalculateVoiceExperience(TimeSpan voiceTime)
        {
            #if DEBUG
                return 100;
            #endif
            const int expPerMinute = 6;
            return (ulong)voiceTime.Minutes * expPerMinute;
        }

        public ulong CalculateChatMessageExperience(ulong messagesCount)
        {
            #if DEBUG
                return 100;
            #endif
            const int minExp = 15;
            const int maxExp = 25;
            return (ulong)Enumerable.Range(0, (int)messagesCount).Sum(x => _random.Next(minExp, maxExp));
        }
    }
#pragma warning restore 162
}