using System.Collections.Generic;

using TaigadevDiscordBot.Core.Bot.Features.Service;

namespace TaigadevDiscordBot.Core.Bot
{
    public interface IBotConfiguration
    {
        string Token { get; }
        string AdminId { get; }

        IList<ulong> WorkOnServerIds { get; }
        string ServiceCategoryName { get; }
        IList<GuildChannel> ServiceChannels { get; }
    }
}