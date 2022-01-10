using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

using TaigadevDiscordBot.Core.Bot.Event.EventArgs;
using TaigadevDiscordBot.Core.Bot.Features;
using TaigadevDiscordBot.Core.Bot.Features.UserActivity;
using TaigadevDiscordBot.Core.Bot.Features.UserExperience;

namespace TaigadevDiscordBot.App.Bot.Features.UserActivity
{
    public class TextActivityService : ITextActivityService
    {
                                // textChannelId / lastMessageAuthorId
        private readonly ConcurrentDictionary<ulong, ulong> _channelsActivity = new();

        private readonly IUserRepository _userRepository;
        private readonly IExperienceCalculationService _experienceCalculationService;
        private readonly IUserLevelService _userLevelService;

        public TextActivityService(IUserRepository userRepository, IExperienceCalculationService experienceCalculationService, IUserLevelService userLevelService)
        {
            _userRepository = userRepository;
            _experienceCalculationService = experienceCalculationService;
            _userLevelService = userLevelService;
        }

        public async ValueTask UpdateUserTextActivityAsync(NewTextMessageEventArgs eventArgs)
        {
            var key = eventArgs.Message.Channel.Id;
            if (!_channelsActivity.TryRemove(key, out var lastAuthorId) 
                || lastAuthorId != eventArgs.User.Id)
            {
                await _userRepository.UpdateUserAsync(eventArgs.User.Id, eventArgs.Guild.Id, user =>
                {
                    user.Nickname = eventArgs.User.Nickname ?? eventArgs.User.Username;
                    // roles without 'everyone' role
                    user.Roles = eventArgs.User.RoleIds.Where(x => x != eventArgs.Guild.Id).ToList();
                    user.Experience += _experienceCalculationService.CalculateChatMessageExperience(1);
                    return Task.CompletedTask;
                });

                await _userLevelService.LevelUpUserIfNeededAsync(eventArgs.User.Id, eventArgs.Guild.Id);
            }

            _channelsActivity.TryAdd(key, eventArgs.User.Id);
        }
    }
}