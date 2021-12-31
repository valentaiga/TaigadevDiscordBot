using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

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
                // todo: change app type to service instead of web, so heroku host will not die
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
                Console.WriteLine($"Unable to start. Exception: {ex}");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var configuration = AppConfigurationExtensions.BuildConfiguration();
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // future web UI
                    webBuilder.UseStartup<Startup>();
                    // startup for heroku app start
                    webBuilder.UseKestrel(options =>
                        options.ListenAnyIP(int.TryParse(Environment.GetEnvironmentVariable("PORT"), out var port) ? port : 5000));
                })
                .ConfigureLogging(configuration)
                .ConfigureServices(configuration)
                .ConfigureHostedServices();
        }
    }
}
