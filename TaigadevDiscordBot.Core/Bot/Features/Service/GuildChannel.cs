using System.Text.Json.Serialization;
using Discord.WebSocket;

namespace TaigadevDiscordBot.Core.Bot.Features.Service
{
    public class GuildChannel
    {
        [JsonConstructor]
        public GuildChannel()
        {
        }

        public GuildChannel(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; set; }

        public string Description { get; set; }
        
        [JsonIgnore]
        public SocketTextChannel Channel { get; set; }

        public bool IsAudit { get; set; }
    }
}