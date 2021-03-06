using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using TaigadevDiscordBot.Core.Bot.Features;
using TaigadevDiscordBot.Core.Bot.Features.Service;
using TaigadevDiscordBot.Core.Bot.Features.UserActivity;
using TaigadevDiscordBot.Core.Bot.Features.UserExperience;

#nullable enable
namespace TaigadevDiscordBot.App.Bot.Features.UserActivity
{
    public class UserActivityUpdateHostService : IHostedService, IDisposable
    {
        private readonly IVoiceActivityService _voiceActivityService;
        private readonly IExperienceService _experienceService;
        private readonly IUserRepository _userRepository;
        private readonly IUserLevelService _userLevelService;
        private readonly IBotMaintainingService _botMaintainingService;
        private readonly ILogger<UserActivityUpdateHostService> _logger;
        private readonly TimeSpan _executionTimespan;
        private Timer _timer = null!;

        public UserActivityUpdateHostService(
            IVoiceActivityService voiceActivityService, 
            IExperienceService experienceService, 
            IUserRepository userRepository,
            IUserLevelService userLevelService,
            IBotMaintainingService botMaintainingService,
            ILogger<UserActivityUpdateHostService> logger)
        {
            _voiceActivityService = voiceActivityService;
            _experienceService = experienceService;
            _userRepository = userRepository;
            _userLevelService = userLevelService;
            _botMaintainingService = botMaintainingService;
            _logger = logger;
            _executionTimespan = TimeSpan.FromSeconds(5);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(ExecuteAsync, null, TimeSpan.Zero, _executionTimespan);
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service stopped");
            _timer.Change(Timeout.Infinite, 0);
            await _botMaintainingService.SaveUsersActivitiesAsync();
        }

        private async void ExecuteAsync(object? state)
        {
            if (_voiceActivityService.ActivitiesToCollectCount == 0)
            {
                return;
            }

            _logger.LogInformation($"Users voice activity/experience update started. Total {_voiceActivityService.ActivitiesToCollectCount} activities");
            foreach (var activity in _voiceActivityService.CollectActivities())
            {
                try
                {
                    var expToAdd = await _experienceService.CalculateVoiceExperienceAsync(activity.UserId, activity.GuildId, activity.TimeInVoiceSpent);
                    if (expToAdd == 0)
                    {
                        continue;
                    }

                    await _userRepository.UpdateUserAsync(activity.UserId, activity.GuildId, user =>
                    {
                        user.TotalVoiceActivity += activity.TimeInVoiceSpent;
                        user.Nickname = activity.Nickname;
                        user.Roles = activity.Roles;
                        user.Experience += expToAdd;
                        return Task.CompletedTask;
                    });

                    await _userLevelService.LevelUpUserIfNeededAsync(activity.UserId, activity.GuildId);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error due user activity update. {ex}");
                    _voiceActivityService.AddActivity(activity);
                }
            }
            _logger.LogInformation($"Users voice activity/experience update finished");
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}