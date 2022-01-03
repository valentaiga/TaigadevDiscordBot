using System.Collections.Generic;
using System.Threading.Tasks;

using TaigadevDiscordBot.Core.Bot.Event.EventArgs;

namespace TaigadevDiscordBot.Core.Bot.Features.UserActivity
{
    public interface IVoiceActivityService
    {
        ValueTask UpdateUserVoiceActivityAsync(VoiceStatusUpdatedEventArgs eventArgs);

        IEnumerable<UserVoiceActivity> CollectActivities();

        void AddActivity(UserVoiceActivity voiceActivity);

        void FinishAllActivities();

        int ActivitiesToCollectCount { get; }
    }
}