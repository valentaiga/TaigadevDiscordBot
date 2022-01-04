using System.Collections.Generic;
using System.Text.Json.Serialization;

using Discord.WebSocket;

namespace TaigadevDiscordBot.Core.Bot.Features.Service
{
    public class GuildChannel
    {
        [JsonConstructor]
        public GuildChannel()
        {
            Channels = new List<SocketTextChannel>();
        }

        public GuildChannel(string name, string description) : this()
        {
            Name = name;
            Description = description;
        }

        public string Name { get; set; }

        public string Description { get; set; }
        
        [JsonIgnore]
        public List<SocketTextChannel> Channels { get; set; } 

        public bool IsAudit { get; set; }
    }
}