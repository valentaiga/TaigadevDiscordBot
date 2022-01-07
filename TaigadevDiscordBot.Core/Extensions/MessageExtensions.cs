using System;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

using TaigadevDiscordBot.Core.Bot;

namespace TaigadevDiscordBot.Core.Extensions
{
    public static class MessageExtensions
    {
        private static readonly TimeSpan DefaultDeleteMessageTimespan = TimeSpan.FromSeconds(5);
        private static readonly Color DefaultColor = Color.Gold;

        public static async Task CommandMessageReplyAsync(this SocketMessage message, string replyText, Embed embedMessage = null, TimeSpan? deleteMessageTimespan = null)
        {
            await message.TryDeleteAsync();
            await message.Channel.SendAndRemoveMessageAsync(replyText, deleteMessageTimespan ?? DefaultDeleteMessageTimespan, embedMessage);
        }

        public static async Task TryDeleteAsync(this IMessage message)
        {
            try { await message.DeleteAsync(); } catch { }
        }

        public static EmbedBuilder AdjustBotFields(this EmbedBuilder embedBuilder, IBotConfiguration botConfiguration, Color? color = null)
            => embedBuilder
                .WithColor(color ?? DefaultColor)
                .WithCurrentTimestamp()
                .WithFooter($"Powered by {botConfiguration.SelfUser.Username}", botConfiguration.SelfUser.GetAvatarUrl(size: 16));
    }
}