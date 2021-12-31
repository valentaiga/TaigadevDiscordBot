using System.Threading.Tasks;

using Discord.WebSocket;

using Microsoft.Extensions.Configuration;

using TaigadevDiscordBot.Core.Initialization;

namespace TaigadevDiscordBot.App.Initialization
{
    public class BotInitializationModule : IInitializationModule
    {
        public BotInitializationModule(IConfiguration configuration)
        {
        }

        public Task InitializeAsync(DiscordSocketClient client)
        {
            return CreateRequiredChannelsAsync(client);
        }

        private async Task CreateRequiredChannelsAsync(DiscordSocketClient client)
        {
            // todo: add connected servers
            // todo: create required channels, set them in channel service for services usage (like audit)
            return;
        }
    }
}