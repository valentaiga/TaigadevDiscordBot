using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using TaigadevDiscordBot.Extensions;

namespace TaigadevDiscordBot
{
    public class Program
    {
        private static readonly CancellationTokenSource TokenSource = new();
        private static IHost _webHost;

        public static async Task Main(string[] args)
        {
            try
            {
                _webHost = CreateHostBuilder(args).Build();
                await _webHost.RunAsync(TokenSource.Token);
            }
            catch (TaskCanceledException)
            {
                // ignore
            }
            catch (Exception ex)
            {
                // log error
                var logger = _webHost.Services.GetRequiredService<ILogger<Program>>();
                logger.LogCritical($"Unable to start. Exception: {ex}");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var configuration = AppConfigurationExtensions.BuildConfiguration();
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // startup for heroku app start
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(configuration)
                .ConfigureServices(configuration)
                .ConfigureHostedServices();
        }
    }
}
