using System.IO;

using Discord.WebSocket;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using TaigadevDiscordBot.App.Bot;
using TaigadevDiscordBot.App.Bot.Features;
using TaigadevDiscordBot.App.Bot.Features.Commands;
using TaigadevDiscordBot.App.Bot.Features.Commands.Fun;
using TaigadevDiscordBot.App.Bot.Features.Commands.Profile;
using TaigadevDiscordBot.App.Bot.Features.UserActivity;
using TaigadevDiscordBot.App.Bot.Features.UserExperience;
using TaigadevDiscordBot.App.Database.Redis;
using TaigadevDiscordBot.App.Initialization;
using TaigadevDiscordBot.Core.Bot;
using TaigadevDiscordBot.Core.Bot.Event;
using TaigadevDiscordBot.Core.Bot.Features;
using TaigadevDiscordBot.Core.Bot.Features.Commands;
using TaigadevDiscordBot.Core.Bot.Features.UserActivity;
using TaigadevDiscordBot.Core.Bot.Features.UserExperience;
using TaigadevDiscordBot.Core.Database.Redis;
using TaigadevDiscordBot.Core.Initialization;

namespace TaigadevDiscordBot.Extensions
{
    public static class AppConfigurationExtensions
    {
        public static IHostBuilder ConfigureServices(this IHostBuilder host, IConfiguration configuration)
            => host.ConfigureServices(services =>
            {
                services.AddSingleton(configuration);

                // util
                services.AddSingleton<IRedisConfiguration, RedisConfiguration>();
                services.AddSingleton<IRedisProvider, RedisProvider>();
                services.AddSingleton<IRedisRepository, RedisRepository>();

                // features
                services.AddSingleton<IUserEventHandler, UserEventHandler>();
                services.AddSingleton<IExperienceCalculationService, ExperienceCalculationService>();
                services.AddSingleton<IUserLevelService, UserLevelService>();
                services.AddSingleton<IUserRepository, RedisUserRepository>();
                services.AddSingleton<IVoiceActivityService, VoiceActivityService>();
                services.AddSingleton<ITextActivityService, TextActivityService>();

                // initialization modules
                services.AddSingleton<IInitializationModule, ServiceChannelsInitialization>();

                // commands
                services.AddSingleton<ICommandService, CommandService>();
                services.AddSingleton<ITextChannelCommand, ChaosVoiceMoveCommand>();
                services.AddSingleton<ITextChannelCommand, GetProfileCommand>();
                
                // client
                services.AddSingleton<DiscordSocketClient>();
                services.AddSingleton<IBotConfiguration, BotConfiguration>();
            });

        public static IHostBuilder ConfigureHostedServices(this IHostBuilder host)
            => host.ConfigureServices(services =>
            {
                services.AddHostedService<BotClient>();
                services.AddHostedService<UserActivityUpdateHostService>();
            });

        public static IConfiguration BuildConfiguration()
            => new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
#if DEBUG
                .AddJsonFile("appsettings.Development.json")
#endif
                .AddEnvironmentVariables()
                .Build();

        public static IHostBuilder ConfigureLogging(this IHostBuilder host, IConfiguration configuration)
            => host.ConfigureLogging(builder =>
            {
                builder.AddConfiguration(configuration);
            });
    }
}