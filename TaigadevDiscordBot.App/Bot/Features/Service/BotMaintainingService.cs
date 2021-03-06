using System.Linq;
using System.Threading.Tasks;

using TaigadevDiscordBot.Core.Bot.Features.Service;
using TaigadevDiscordBot.Core.Bot.Features.UserActivity;
using TaigadevDiscordBot.Core.Database.Redis;

namespace TaigadevDiscordBot.App.Bot.Features.Service
{
    public class BotMaintainingService : IBotMaintainingService
    {
        const string CacheKey = "activityList";

        private readonly IVoiceActivityService _voiceActivityService;
        private readonly IRedisProvider _redisProvider;

        public BotMaintainingService(IVoiceActivityService voiceActivityService, IRedisProvider redisProvider)
        {
            _voiceActivityService = voiceActivityService;
            _redisProvider = redisProvider;
        }

        public async Task SaveUsersActivitiesAsync()
        {
            var activities = _voiceActivityService.CollectActivities().ToArray();
            if (activities.Length > 0)
            {
                await _redisProvider.SetAddAsync(CacheKey, activities);
            }
        }

        public async Task ProcessSavedUsersActivitiesAsync()
        {
            var activitiesList = await _redisProvider.PopSetAsync<UserVoiceActivity>(CacheKey);
            Parallel.ForEach(activitiesList, activity =>
            {
                _voiceActivityService.AddActivity(activity);
            });
        }
    }
}