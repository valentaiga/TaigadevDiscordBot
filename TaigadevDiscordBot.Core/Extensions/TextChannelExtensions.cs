using System;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

namespace TaigadevDiscordBot.Core.Extensions
{
    public static class TextChannelExtensions
    {
        public static async Task SendAndRemoveMessageAsync(this ISocketMessageChannel channel, string text, TimeSpan deleteTimespan, Embed embedMessage = null)
        {
            var message = await channel.SendMessageAsync(text, embed: embedMessage);

            if (deleteTimespan != TimeSpan.MaxValue)
            {
                await Task.Delay(deleteTimespan);
                await message.DeleteAsync();
            }
        }
    }
}