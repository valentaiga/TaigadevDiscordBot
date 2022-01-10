using System.IO;

using Discord.WebSocket;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using TaigadevDiscordBot.App.Bot;
using TaigadevDiscordBot.App.Bot.Features;
using TaigadevDiscordBot.App.Bot.Features.Collectors;
using TaigadevDiscordBot.App.Bot.Features.Commands;
using TaigadevDiscordBot.App.Bot.Features.Commands.Fun;
using TaigadevDiscordBot.App.Bot.Features.Commands.Profile;
using TaigadevDiscordBot.App.Bot.Features.Commands.Tops;
using TaigadevDiscordBot.App.Bot.Features.Commands.Unspecified;
using TaigadevDiscordBot.App.Bot.Features.Service;
using TaigadevDiscordBot.App.Bot.Features.UserActivity;
using TaigadevDiscordBot.App.Bot.Features.UserExperience;
using TaigadevDiscordBot.App.Database.Redis;
using TaigadevDiscordBot.App.Initialization;
using TaigadevDiscordBot.Core.Bot;
using TaigadevDiscordBot.Core.Bot.Event;
using TaigadevDiscordBot.Core.Bot.Features;
using TaigadevDiscordBot.Core.Bot.Features.Collectors;
using TaigadevDiscordBot.Core.Bot.Features.Commands;
using TaigadevDiscordBot.Core.Bot.Features.Service;
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
                services.AddSingleton<IBotMaintainingService, BotMaintainingService>();
                services.AddSingleton<IEmojiCounterService, EmojiCounterService>();
                services.AddSingleton<IGuildRepository, GuildRepository>();
                services.AddSingleton<IRolesService, RolesService>();
                services.AddSingleton<IAuditLogger, AuditLogger>();
                services.AddSingleton<IPersonalAuditLogger, PersonalAuditLogger>();

                // initialization modules
                services.AddSingleton<IInitializationModule, ServiceChannelsInitialization>();
                services.AddSingleton<IInitializationModule, UserActivityInitialization>();

                // commands
                services.AddSingleton<ICommandService, CommandService>();
                services.AddSingleton<ICommand, ChaosVoiceMoveCommand>();
                services.AddSingleton<ICommand, GetProfileCommand>();
                services.AddSingleton<ICommand, HelpCommand>();
                services.AddSingleton<ICommand, SetLevelCommand>();
                services.AddSingleton<ICommand, ClownsTopCommand>();
                services.AddSingleton<ICommand, CookiesTopCommand>();
                services.AddSingleton<ICommand, IgnoreChannelCommand>();
                services.AddSingleton<ICommand, UnignoreChannelCommand>();
                services.AddSingleton<ICommand, AddJoinRolesCommand>();
                services.AddSingleton<ICommand, RemoveJoinRolesCommand>();
                services.AddSingleton<ICommand, TrackUserCommand>();
                services.AddSingleton<ICommand, UserAuditCommand>();

                // client
                services.AddSingleton(_ =>
                {
                    var config = new DiscordSocketConfig
                    {
                        AlwaysDownloadUsers = true,
                        MessageCacheSize = 100,
                    };
                    return new DiscordSocketClient(config);
                });
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