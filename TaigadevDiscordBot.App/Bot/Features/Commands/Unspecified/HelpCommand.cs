using System;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

using Microsoft.Extensions.DependencyInjection;

using TaigadevDiscordBot.Core.Bot.Features.Commands;
using TaigadevDiscordBot.Core.Extensions;

namespace TaigadevDiscordBot.App.Bot.Features.Commands.Unspecified
{
    public class HelpCommand : CommandBase
    {
        private readonly IServiceProvider _serviceProvider;

        public HelpCommand(IServiceProvider serviceProvider)
            : base(
                "help", 
                "Get list of all commands", 
                "t!help", 
                false,
                GuildPermission.SendMessages)
        {
            _serviceProvider = serviceProvider;
        }
        
        public override Task ExecuteAsync(SocketMessage message, SocketGuild guild)
        {
            var embedBuilder = new EmbedBuilder()
                .WithColor(Color.Gold);
            foreach (var command in _serviceProvider.GetServices<ICommand>())
            {
                embedBuilder.AddField(command.Command, command.UsageExample);
            }

            return message.CommandMessageReplyAsync(message.Author.Mention, embedBuilder.Build(), TimeSpan.MaxValue);
        }
    }
}