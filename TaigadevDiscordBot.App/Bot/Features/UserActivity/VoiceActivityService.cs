using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

using TaigadevDiscordBot.Core.Bot.Event;
using TaigadevDiscordBot.Core.Bot.Event.EventArgs;
using TaigadevDiscordBot.Core.Bot.Features;
using TaigadevDiscordBot.Core.Bot.Features.UserActivity;

namespace TaigadevDiscordBot.App.Bot.Features.UserActivity
{
    public class VoiceActivityService : IVoiceActivityService
    {
                                    // userCacheKey / UserVoiceActivity 
        private readonly ConcurrentDictionary<string, UserVoiceActivity> _usersActivity;
        // contains activities to collect
        private readonly ConcurrentQueue<UserVoiceActivity> _userActivitiesToCollect;

        public VoiceActivityService()
        {
            _usersActivity = new();
            _userActivitiesToCollect = new();
        }

        public ValueTask UpdateUserVoiceActivityAsync(VoiceStatusUpdatedEventArgs eventArgs)
        {
            var key = User.GetCacheKey(eventArgs.Guild.Id, eventArgs.User.Id);
            switch (eventArgs.VoiceStatus)
            {
                case UserVoiceStatus.Muted:
                case UserVoiceStatus.Left:
                    if (_usersActivity.TryRemove(key, out var value))
                    {
                        UpdateActivity(value);
                        _userActivitiesToCollect.Enqueue(value);
                    }
                    break;
                case UserVoiceStatus.Unmuted:
                    if (!_usersActivity.TryGetValue(key, out _))
                    {
                        var activity = new UserVoiceActivity(eventArgs.User.Id, eventArgs.Guild.Id, eventArgs.User.Username);
                        _usersActivity.TryAdd(key, activity);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unexpected {nameof(UserVoiceStatus)} status.");
            }

            return ValueTask.CompletedTask;
        }

        public IEnumerable<UserVoiceActivity> CollectActivities()
        {
            while (_userActivitiesToCollect.TryDequeue(out var activity))
            {
                yield return activity;
            }
        }

        public int ActivitiesToCollectCount => _userActivitiesToCollect.Count;

        public void AddActivity(UserVoiceActivity voiceActivity)
            => _userActivitiesToCollect.Enqueue(voiceActivity);

        private static void UpdateActivity(UserVoiceActivity userVoiceActivity)
        {
            var dtNow = DateTime.UtcNow;
            var delta = dtNow - userVoiceActivity.LastActivityDateTime;
            userVoiceActivity.TimeInVoiceSpent = userVoiceActivity.TimeInVoiceSpent.Add(delta);
            userVoiceActivity.LastActivityDateTime = dtNow;
        }
    }
}