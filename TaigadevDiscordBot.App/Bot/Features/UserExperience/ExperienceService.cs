using System;
using System.Threading.Tasks;

using TaigadevDiscordBot.Core.Bot.Features.Service;
using TaigadevDiscordBot.Core.Bot.Features.UserExperience;

namespace TaigadevDiscordBot.App.Bot.Features.UserExperience
{
#pragma warning disable 162
    public class ExperienceService : IExperienceService
    {
        private readonly Random _random = new();
        private readonly IGuildRepository _guildRepository;

        public ExperienceService(IGuildRepository guildRepository)
        {
            _guildRepository = guildRepository;
        }

        public async Task<ulong> CalculateVoiceExperienceAsync(ulong userId, ulong guildId, TimeSpan voiceTime)
        {
            if (await IsUserProhibitedAsync(userId, guildId))
            {
                return 0;
            }

            #if DEBUG
            return 100;
            #endif
            const int expPerMinute = 6;
            return (ulong)voiceTime.Minutes * expPerMinute;
        }

        public async Task<ulong> CalculateMessageExperienceAsync(ulong userId, ulong guildId)
        {
            if (await IsUserProhibitedAsync(userId, guildId))
            {
                return 0;
            }

            #if DEBUG
                return 100;
            #endif
            const int minExp = 15;
            const int maxExp = 25;
            return (ulong)(_random.Next(minExp, maxExp));
        }

        public Task ProhibitExperienceGainAsync(ulong userId, ulong guildId)
        {
            return _guildRepository.UpdateGuildAsync(guildId, guild =>
            {
                if (!guild.ProhibitExperienceUsers.Contains(userId))
                {
                    guild.ProhibitExperienceUsers.Add(userId);
                }
                return Task.CompletedTask;
            });
        }

        public Task AllowExperienceGainAsync(ulong userId, ulong guildId)
        {
            return _guildRepository.UpdateGuildAsync(guildId, guild =>
            {
                if (!guild.ProhibitExperienceUsers.Contains(userId))
                {
                    guild.ProhibitExperienceUsers.Remove(userId);
                }
                return Task.CompletedTask;
            });
        }


        private async Task<bool> IsUserProhibitedAsync(ulong userId, ulong guildId)
        {
            var guild = await _guildRepository.GetGuildAsync(guildId);
            return guild.ProhibitExperienceUsers.Contains(userId);
        }
    }
#pragma warning restore 162
}