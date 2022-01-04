using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

using TaigadevDiscordBot.Core.Bot;
using TaigadevDiscordBot.Core.Bot.Features.Service;

namespace TaigadevDiscordBot.App.Bot.Features.Service
{
    public class AuditLogger : IAuditLogger
    {
        private readonly IBotConfiguration _botConfiguration;

        public AuditLogger(IBotConfiguration botConfiguration)
        {
            _botConfiguration = botConfiguration;
        }

        public Task LogErrorAsync(Exception exception, ulong guildId)
        {
            var auditChannel = GetAuditChannel(guildId);
            var embedMessage = new EmbedBuilder()
                .AddField("ErrorMessage", exception.Message)
                .AddField("Source", exception.Source ?? "<none>")
                .AddField("Inner exception", exception.InnerException?.ToString() ?? "<none>")
                .WithCurrentTimestamp()
                .WithColor(Color.Red)
                .Build();
            return auditChannel.SendMessageAsync($"**Unexpected error:** {Environment.NewLine}```{exception.StackTrace}```", embed: embedMessage);
        }

        public Task LogInformationAsync(string message, ulong guildId, IDictionary<string, string> embedMessageFields = null)
        {
            var auditChannel = GetAuditChannel(guildId);

            var embedMessage = (Embed)null;
            if (embedMessageFields is not null)
            {
                var embedMessageBuilder = new EmbedBuilder()
                    .WithColor(Color.Gold);

                foreach (var keyValuePair in embedMessageFields)
                {
                    embedMessageBuilder.AddField(keyValuePair.Key, keyValuePair.Value);
                }

                embedMessage = embedMessageBuilder.Build();
            }

            return auditChannel.SendMessageAsync(message, embed: embedMessage);
        }

        private SocketTextChannel GetAuditChannel(ulong guildId) => _botConfiguration.ServiceChannels.Find(x => x.IsAudit)!.Channels.Find(x => x.Guild.Id == guildId);
    }
}