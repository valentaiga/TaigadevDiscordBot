using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

using TaigadevDiscordBot.Core.Bot.Event.EventArgs;
using TaigadevDiscordBot.Core.Bot.Features.Collectors;
using TaigadevDiscordBot.Core.Constants;
using TaigadevDiscordBot.Core.Database.Redis;

namespace TaigadevDiscordBot.App.Bot.Features.Collectors
{
    public class EmojiCounterService : IEmojiCounterService
    {
        private readonly IRedisProvider _redisProvider;
        private readonly ConcurrentQueue<string> _lastReactionKeys = new();
                                            // emoji / masterKey
        private readonly ConcurrentDictionary<string, string> _reactionsToTrack;

        public EmojiCounterService(IRedisProvider redisProvider)
        {
            _redisProvider = redisProvider;
            _reactionsToTrack = new();
            _reactionsToTrack.TryAdd(Emojis.CookieEmote, "cookiesCollection");
            _reactionsToTrack.TryAdd(Emojis.ClownEmote, "clownsCollection");
        }

        public async ValueTask IncrementUserEmojiCount(ReactionAddedEventArgs eventArgs)
        {
            var lastReactedUsers = _lastReactionKeys.ToImmutableHashSet();
            var reactionKey = GetLastReactionKey();
            if (_reactionsToTrack.TryGetValue(eventArgs.Reaction.Emote.Name, out var masterKey)
                && !eventArgs.Message.Author.IsBot
                && !lastReactedUsers.Contains(reactionKey))
            {
                var outerKey = GetOuterKey(eventArgs.TextChannel.Guild.Id, masterKey);
                var innerKey = eventArgs.Message.Author.Id.ToString();
                var currentValue = await _redisProvider.GetFromHashAsync<int>(outerKey, innerKey);
                await _redisProvider.AddToHashAsync(outerKey, innerKey, ++currentValue);
                _lastReactionKeys.Enqueue(reactionKey);

                if (_lastReactionKeys.Count > 20)
                {
                    _lastReactionKeys.TryDequeue(out _);
                }
            }

            string GetLastReactionKey() => $"{eventArgs.Message.Id}:{eventArgs.Reaction.UserId}";
        }

        public Task<int> GetCurrentUserCount(ulong userId, ulong guildId, string emoji)
        {
            _reactionsToTrack.TryGetValue(emoji, out var masterKey);
            var outerKey = GetOuterKey(guildId, masterKey);
            return _redisProvider.GetFromHashAsync<int>(outerKey, userId.ToString());
        }

        public async Task<Dictionary<ulong, int>> GetCurrentGuildTop(ulong guildId, string emoji)
        {
            _reactionsToTrack.TryGetValue(emoji, out var masterKey);
            var outerKey = GetOuterKey(guildId, masterKey);
            return (await _redisProvider.GetFromHashAllAsync<int>(outerKey))
                .OrderByDescending(x => x.Value)
                .Take(10)
                .ToDictionary(x => ulong.Parse(x.Key), x => x.Value);
        }

        private static string GetOuterKey(ulong guildId, string masterKey)
            => $"{guildId}:{masterKey}";
    }
}