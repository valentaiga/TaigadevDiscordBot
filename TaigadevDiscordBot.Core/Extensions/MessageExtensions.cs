using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace TaigadevDiscordBot.Core.Extensions
{
    public static class MessageExtensions
    {
        private static readonly TimeSpan DefaultDeleteMessageTimespan = TimeSpan.FromSeconds(5);

        public static async Task CommandMessageReplyAsync(this SocketMessage message, string replyText, Embed embedMessage = null)
        {
            await message.DeleteAsync();
            await message.Channel.SendAndRemoveMessageAsync(replyText, DefaultDeleteMessageTimespan, embedMessage);
        }
    }
}