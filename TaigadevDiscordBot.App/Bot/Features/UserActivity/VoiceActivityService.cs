using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;

using TaigadevDiscordBot.Core.Bot.Event;
using TaigadevDiscordBot.Core.Bot.Event.EventArgs;
using TaigadevDiscordBot.Core.Bot.Features;
using TaigadevDiscordBot.Core.Bot.Features.UserActivity;

namespace TaigadevDiscordBot.App.Bot.Features.UserActivity
{
    public class VoiceActivityService : IVoiceActivityService
    {
                                                // voiceChannelId, userId / userVoiceActivity
        private readonly ConcurrentDictionary<ulong, ConcurrentDictionary<ulong, UserVoiceActivity>> UsersActivity = new();

                                    // userCacheKey / UserVoiceActivity 
        private readonly ConcurrentDictionary<string, UserVoiceActivity> _usersActivity = new();
        // contains activities to collect
        private readonly ConcurrentQueue<UserVoiceActivity> _userActivitiesToCollect = new();

        public ValueTask UpdateUserVoiceActivityAsync(VoiceStatusUpdatedEventArgs eventArgs)
        {
            var dtNow = DateTime.UtcNow;
            var currentUserId = eventArgs.User.Id;

            if (eventArgs.CurrentChannel is null)
            {
                ProcessLeftUser();
                return ValueTask.CompletedTask;
            }

            if (eventArgs.PreviousChannel is null)
            {
                ProcessJoinedUser();
                return ValueTask.CompletedTask;
            }

            // todo: process muted users

            ProcessMovedUser();
            return ValueTask.CompletedTask;

            ConcurrentDictionary<ulong, UserVoiceActivity> GetUsersInVoiceChannel(ulong voiceChannelId)
            {
                if (!UsersActivity.TryGetValue(voiceChannelId, out var result))
                {
                    result = new();
                    UsersActivity.TryAdd(voiceChannelId, result);
                }

                return result;
            }

            void ProcessLeftUser()
            {
                // finish current user activity
                var previousChannelId = eventArgs.PreviousChannel!.Id;
                var usersInChannel = GetUsersInVoiceChannel(previousChannelId);
                if (usersInChannel.TryRemove(currentUserId, out var activity))
                {
                    _userActivitiesToCollect.Enqueue(UpdateActivity(activity));
                }

                if (usersInChannel.Count == 1)
                {
                    // finish last person activity in channel
                    _userActivitiesToCollect.Enqueue(UpdateActivity(usersInChannel.Values.First()));
                    UsersActivity.TryRemove(previousChannelId, out _);
                }
            }

            void ProcessJoinedUser()
            {
                var currentChannelId = eventArgs.CurrentChannel!.Id;
                var usersInChannel = GetUsersInVoiceChannel(currentChannelId);

                if (usersInChannel.Count == 1)
                {
                    // update enter time for currently in channel channel user
                    var key = usersInChannel.Keys.First();
                    usersInChannel.TryRemove(key, out var user);
                    user!.VoiceEnterDateTime = dtNow;
                    usersInChannel.TryAdd(key, user);
                }

                // start new current user activity
                var activity = new UserVoiceActivity(eventArgs.User.Id, eventArgs.Guild.Id, eventArgs.User.Username);
                usersInChannel.TryAdd(currentUserId, activity);
            }

            void ProcessMovedUser()
            {
                // remove old voice value, update voice channel in activity
                // check for activities in old channel and new channel (second condition)
                var previousChannelId = eventArgs.PreviousChannel!.Id;
                var currentChannelId = eventArgs.CurrentChannel!.Id;

                // remove old voice value
                var usersInChannel = GetUsersInVoiceChannel(previousChannelId);
                if (usersInChannel.TryRemove(currentUserId, out var activity))
                {
                    if (usersInChannel.Count == 0)
                    {
                        activity.VoiceEnterDateTime = dtNow;
                    }

                    if (usersInChannel.Count == 1)
                    {
                        // finish last person activity in channel
                        _userActivitiesToCollect.Enqueue(UpdateActivity(usersInChannel.Values.First()));
                    }

                    if (usersInChannel.Count < 2)
                    {
                        UsersActivity.TryRemove(previousChannelId, out _);
                    }
                }

                // update new voice value
                activity ??= new UserVoiceActivity(eventArgs.User.Id, eventArgs.Guild.Id, eventArgs.User.Username);
                usersInChannel = GetUsersInVoiceChannel(currentChannelId);
                if (usersInChannel.Count == 1)
                {
                    var key = usersInChannel.Keys.First();
                    usersInChannel.TryRemove(key, out var user);
                    user!.VoiceEnterDateTime = dtNow;
                    usersInChannel.TryAdd(key, user);
                }
                usersInChannel.TryAdd(currentUserId, activity);
            }
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

        private static UserVoiceActivity UpdateActivity(UserVoiceActivity userVoiceActivity)
        {
            var dtNow = DateTime.UtcNow;
            var delta = dtNow - userVoiceActivity.VoiceEnterDateTime;
            userVoiceActivity.TimeInVoiceSpent = userVoiceActivity.TimeInVoiceSpent.Add(delta);
            userVoiceActivity.VoiceEnterDateTime = dtNow;
            return userVoiceActivity;
        }
    }
}