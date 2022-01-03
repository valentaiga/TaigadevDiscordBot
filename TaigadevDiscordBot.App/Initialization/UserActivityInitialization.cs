using System.Threading.Tasks;
using Discord.WebSocket;
using TaigadevDiscordBot.Core.Bot.Features.Service;
using TaigadevDiscordBot.Core.Initialization;

namespace TaigadevDiscordBot.App.Initialization
{
    public class UserActivityInitialization : IInitializationModule
    {
        private readonly IBotMaintainingService _botMaintainingService;

        public UserActivityInitialization(IBotMaintainingService botMaintainingService)
        {
            _botMaintainingService = botMaintainingService;
        }

        public Task InitializeAsync(DiscordSocketClient client)
        {
            return _botMaintainingService.ProcessSavedUsersActivitiesAsync();
        }
    }
}