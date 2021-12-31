using System;
using System.Threading.Tasks;

using Discord.WebSocket;

namespace TaigadevDiscordBot.Core.Extensions
{
    public static class TextChannelExtensions
    {
        public static async Task SendAndRemoveMessageAsync(this ISocketMessageChannel channel, string text, TimeSpan deleteTimespan)
        {
            var message = await channel.SendMessageAsync(text);
            await Task.Delay(deleteTimespan);
            await message.DeleteAsync();
        }
    }
}