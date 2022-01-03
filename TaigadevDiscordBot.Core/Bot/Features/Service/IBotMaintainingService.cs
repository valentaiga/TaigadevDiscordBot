using System.Threading.Tasks;

namespace TaigadevDiscordBot.Core.Bot.Features.Service
{
    public interface IBotMaintainingService
    {
        Task SaveUsersActivitiesAsync();

        Task ProcessSavedUsersActivitiesAsync();
    }
}