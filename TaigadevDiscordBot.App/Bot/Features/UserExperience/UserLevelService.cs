using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Discord.WebSocket;

using Microsoft.Extensions.Logging;

using TaigadevDiscordBot.Core.Bot.Features;
using TaigadevDiscordBot.Core.Bot.Features.UserExperience;

namespace TaigadevDiscordBot.App.Bot.Features.UserExperience
{
    public class UserLevelService : IUserLevelService
    {
                                            // lvl / exp
        private readonly ConcurrentDictionary<int, ulong> _levels = new();
        private readonly Regex _roleLevelRegex = new(@"^([0-9][0-9]?\.)|(100\.)$");

        private readonly IUserRepository _userRepository;
        private readonly DiscordSocketClient _client;
        private readonly ILogger<UserLevelService> _logger;

        private const int MaxLevel = 300;

        public UserLevelService(IUserRepository userRepository, DiscordSocketClient client, ILogger<UserLevelService> logger)
        {
            _userRepository = userRepository;
            _client = client;
            _logger = logger;

            ParallelEnumerable.Range(0, MaxLevel).ForAll(x => _levels.TryAdd(x, CalculateLevelExperience(x)));

            ulong CalculateLevelExperience(int level) => (ulong)(level * level) * 5 + (ulong)level * 50 + 100;
        }

        public async Task LevelUpUserIfNeeded(ulong userId, ulong guildId)
        {
            var user = await _userRepository.GetOrCreateUserAsync(userId, guildId);

            if (IsRoleUpdateAvailable(user.Experience, user.Level))
            {
                await LevelUpUserAsync(userId, guildId);
            }
        }

        private bool IsRoleUpdateAvailable(ulong currentExperience, int currentLevel)
        {
            if (currentLevel == MaxLevel)
            {
                return false;
            }

            var requiredExperience = _levels[currentLevel + 1];
            return currentExperience > requiredExperience;
        }

        public Task LevelUpUserAsync(ulong userId, ulong guildId)
        {
            return _userRepository.UpdateUserAsync(userId, guildId, async user =>
            {
                if (user.Level == MaxLevel)
                {
                    return;
                }
                
                // migration from jupiter bot activity
                await MigrateUserLevelAsync(user, userId, guildId);

                // uncomment if experience should be removed after level-up
                //var requiredExperience = _levels[user.Level++];
                //user.Experience = user.Experience < requiredExperience ? 0 : user.Experience - requiredExperience;

                var guild = _client.GetGuild(guildId);
                var dsUser = guild.GetUser(userId);
                user.Level++;
                
                var nextLevelRole = await guild.Roles.ToAsyncEnumerable().FirstOrDefaultAsync(x => x.Name.StartsWith($"{user.Level}."));

                // next level has no role
                if (nextLevelRole is null)
                {
                    _logger.LogInformation($"Level '{user.Level}' role does not exist. User '{user.Username}' leveled up anyway");
                    return;
                }

                var levelRoles = await dsUser.Roles.ToAsyncEnumerable()
                    .Where(x => _roleLevelRegex.IsMatch(x.Name)).Select(x => x.Id).ToArrayAsync();

                // after migration next role is same as current
                if (!levelRoles.Contains(nextLevelRole.Id))
                {
                    await dsUser.AddRoleAsync(nextLevelRole.Id);
                    await dsUser.RemoveRolesAsync(levelRoles);
                }

                _logger.LogInformation($"User level updated to '{nextLevelRole.Name}'");
            });
        }

        private async Task MigrateUserLevelAsync(User user, ulong userId, ulong guildId)
        {
            if (user.Level != 0)
            {
                return;
            }

            var guild = _client.GetGuild(guildId);
            var dsUser = guild.GetUser(userId);

            var (level, role) = await dsUser.Roles.ToAsyncEnumerable()
                .Where(x => _roleLevelRegex.IsMatch(x.Name))
                .Select(x => (int.Parse(x.Name.Split(".")[0]), x))
                .OrderByDescending(x => x.Item1)
                .FirstOrDefaultAsync();

            if (role is null || level == 0)
            {
                return;
            }

            user.Level = level;
            user.Experience = _levels[user.Level--];
        }
    }
}