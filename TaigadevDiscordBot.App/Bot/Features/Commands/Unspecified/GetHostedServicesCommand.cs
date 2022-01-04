using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TaigadevDiscordBot.Core.Extensions;

namespace TaigadevDiscordBot.App.Bot.Features.Commands.Unspecified
{
    public class GetHostedServicesCommand : CommandBase
    {
        private readonly IServiceProvider _serviceProvider;

        public GetHostedServicesCommand(IServiceProvider serviceProvider) 
            : base("services", "Currently working services", "t!services", false)
        {
            _serviceProvider = serviceProvider;
        }

        public override Task ExecuteAsync(SocketMessage message, SocketGuild guild)
        {
            var embedBuilder = new EmbedBuilder()
                .WithColor(Color.DarkMagenta)
                .WithCurrentTimestamp();

            var hostedServices = _serviceProvider.GetServices<IHostedService>();

            foreach (var hostedService in hostedServices)
            {
                var type = hostedService.GetType();
                if (type.Namespace.StartsWith("TaigadevDiscordBot"))
                {
                    embedBuilder.AddField(type.Namespace, type.Name);
                }
            }

            return message.CommandMessageReplyAsync(null, embedBuilder.Build());
        }
    }
}