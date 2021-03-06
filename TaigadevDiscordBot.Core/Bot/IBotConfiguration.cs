using System.Collections.Generic;

using Discord.WebSocket;

using TaigadevDiscordBot.Core.Bot.Features.Service;

namespace TaigadevDiscordBot.Core.Bot
{
    public interface IBotConfiguration
    {
        string Token { get; }
        string AdminId { get; }
        string Prefix { get; }

        void SetSelfUser(SocketSelfUser selfUser);

        IList<ulong> WorkOnServerIds { get; }
        string ServiceCategoryName { get; }
        List<GuildChannel> ServiceChannels { get; }
        SocketSelfUser SelfUser { get; }
    }
}