using System;
using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

using Microsoft.Extensions.Logging;

using TaigadevDiscordBot.Core.Bot;
using TaigadevDiscordBot.Core.Bot.Features.Service;
using TaigadevDiscordBot.Core.Initialization;

namespace TaigadevDiscordBot.App.Initialization
{
    public class ServiceChannelsInitialization : IInitializationModule
    {
        private readonly IBotConfiguration _botConfiguration;
        private readonly ILogger<ServiceChannelsInitialization> _logger;

        public ServiceChannelsInitialization(IBotConfiguration botConfiguration, ILogger<ServiceChannelsInitialization> logger)
        {
            _botConfiguration = botConfiguration;
            _logger = logger;
        }

        public Task InitializeAsync(DiscordSocketClient client)
        {
            return CreateRequiredChannelsAsync(client);
        }

        private async Task CreateRequiredChannelsAsync(DiscordSocketClient client)
        {
            await foreach (var guild in client.Guilds.ToAsyncEnumerable())
            {
                if (!_botConfiguration.WorkOnServerIds.Contains(guild.Id))
                {
                    _logger.LogInformation($"Server with name '{guild.Name}', id '{guild.Id}' skipped to create channels");
                    continue;
                }

                var serviceCategory = await GetOrCreateServiceCategoryAsync(guild, _botConfiguration.ServiceCategoryName);
                foreach (var channel in _botConfiguration.ServiceChannels)
                {
                    await TryCreateServiceTextChannelAsync(serviceCategory, channel);
                }
            }
        }

        private async Task<SocketCategoryChannel> GetOrCreateServiceCategoryAsync(SocketGuild guild, string categoryName)
        {
            var result = await guild.CategoryChannels.ToAsyncEnumerable().FirstOrDefaultAsync(x => x.Name == categoryName);
            if (result is not null)
            {
                return result;
            }

            var restCategory = await guild.CreateCategoryChannelAsync(categoryName, prop =>
            {
                prop.Position = guild.CategoryChannels.Count;
            });
            var viewPermissions = new OverwritePermissions(viewChannel: PermValue.Allow, readMessageHistory: PermValue.Allow);
            await restCategory.AddPermissionOverwriteAsync(guild.EveryoneRole, viewPermissions);
            _logger.LogInformation($"Category '{categoryName}' successfully created on '{guild.Name}' server with id '{guild.Id}'.");
            return guild.GetCategoryChannel(restCategory.Id);
        }

        private async Task TryCreateServiceTextChannelAsync(SocketCategoryChannel serviceCategory, GuildChannel serviceChannel)
        {
            var result = serviceCategory.Channels.FirstOrDefault(x => x.Name.Equals(serviceChannel.Name, StringComparison.InvariantCultureIgnoreCase)) as SocketTextChannel;
            if (result is null)
            {
                var restChannel = await serviceCategory.Guild.CreateTextChannelAsync(serviceChannel.Name, prop =>
                {
                    prop.CategoryId = serviceCategory.Id;
                    prop.Topic = serviceChannel.Description;
                });
                _logger.LogInformation($"Channel '{serviceChannel.Name}' successfully created on '{serviceCategory.Guild.Name}' server with id '{serviceCategory.Guild.Id}'.");
                result = serviceCategory.Guild.GetTextChannel(restChannel.Id);
            }

            serviceChannel.Channels.Add(result);
        }
    }
}