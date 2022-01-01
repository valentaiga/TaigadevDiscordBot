using System.Threading.Tasks;

using Discord.WebSocket;

namespace TaigadevDiscordBot.Core.Initialization
{
    public interface IInitializationModule
    {
        Task InitializeAsync(DiscordSocketClient client);
    }
}