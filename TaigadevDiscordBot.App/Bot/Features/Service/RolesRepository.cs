using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord.WebSocket;
using TaigadevDiscordBot.Core.Bot.Features.Service;
using TaigadevDiscordBot.Core.Bot.Features.UserExperience;

namespace TaigadevDiscordBot.App.Bot.Features.Service
{
    public class RolesRepository : IRolesRepository
    {
        private readonly Regex _roleLevelRegex = new(@"^([0-9][0-9]?\.)|(100\.)");
        private readonly ConcurrentDictionary<ulong, Dictionary<ulong, CachedRole>> _cachedRoles = new();
        
        private readonly DiscordSocketClient _client;

        public RolesRepository(DiscordSocketClient client)
        {
            _client = client;
        }

        public CachedRole[] GetRoles(ulong guildId, IReadOnlyCollection<ulong> roleIds)
        {
            if (!_cachedRoles.TryGetValue(guildId, out var roles))
            {
                roles = UpdateCachedRoles(guildId);
            }

            var result = roleIds
                .Select(x => roles.TryGetValue(x, out var r) ? r : null)
                .Where(x => x is not null)
                .ToArray();

            return result;
        }

        public void RemoveCachedRoles(ulong guildId)
            => _cachedRoles.TryRemove(guildId, out _);

        private Dictionary<ulong, CachedRole> UpdateCachedRoles(ulong guildId)
        {
            var guild = _client.GetGuild(guildId);

            if (guild is null)
                throw new Exception($"Guild with id '{guildId}' is not found");

            var roles = guild.Roles
                .Where(x => !x.IsEveryone)
                .Select(x => CachedRole.From(x, _roleLevelRegex.IsMatch(x.Name)))
                .ToDictionary(x => x.Id, x => x);

            _cachedRoles.AddOrUpdate(
                guildId,
                _ => roles,
                (_, _) => roles);

            return roles;
        }
    }
}