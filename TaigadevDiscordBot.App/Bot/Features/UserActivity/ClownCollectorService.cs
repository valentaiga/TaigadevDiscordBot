using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

using TaigadevDiscordBot.Core.Bot.Event.EventArgs;
using TaigadevDiscordBot.Core.Bot.Features.UserActivity;
using TaigadevDiscordBot.Core.Database.Redis;

namespace TaigadevDiscordBot.App.Bot.Features.UserActivity
{
    public class ClownCollectorService : IClownCollectorService
    {
        private const string CacheMasterKey = "clownsCollection";
        private const string ClownEmote = @"🤡";
        private readonly IRedisProvider _redisProvider;
        private readonly ConcurrentQueue<string> _lastReactionKeys = new();

        public ClownCollectorService(IRedisProvider redisProvider)
        {
            _redisProvider = redisProvider;
        }

        public async ValueTask IncrementUpdateUserClowns(ReactionAddedEventArgs eventArgs)
        {
            var lastReactedUsers = _lastReactionKeys.ToImmutableHashSet();
            var reactionKey = GetLastReactionKey();
            if (!eventArgs.Message.Author.IsBot
                && eventArgs.Reaction.Emote.Name == ClownEmote
                && !lastReactedUsers.Contains(reactionKey))
            {
                var outerKey = GetOuterKey(eventArgs.TextChannel.Guild.Id);
                var innerKey = eventArgs.Message.Author.Id.ToString();
                var currentValue = await _redisProvider.GetFromHashAsync<int>(outerKey, innerKey);
                await _redisProvider.AddToHashAsync(outerKey, innerKey, ++currentValue);
                _lastReactionKeys.Enqueue(reactionKey);

                if (_lastReactionKeys.Count > 10)
                {
                    _lastReactionKeys.TryDequeue(out _);
                }
            }

            string GetLastReactionKey() => $"{eventArgs.Message.Id}:{eventArgs.Reaction.UserId}";
        }

        public Task<int> GetCurrentUserCount(ulong userId, ulong guildId)
        {
            var outerKey = GetOuterKey(guildId);
            return _redisProvider.GetFromHashAsync<int>(outerKey, userId.ToString());
        }

        public async Task<Dictionary<ulong, int>> GetCurrentGuildTop(ulong guildId)
        {
            var outerKey = GetOuterKey(guildId);
            return (await _redisProvider.GetFromHashAllAsync<int>(outerKey))
                .OrderByDescending(x => x.Value)
                .ToDictionary(x => ulong.Parse(x.Key), x => x.Value);
        }

        private static string GetOuterKey(ulong guildId)
            => $"{guildId}:{CacheMasterKey}";
    }
}