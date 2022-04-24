using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

using Microsoft.Extensions.Logging;

using TaigadevDiscordBot.Core.Bot.Features;
using TaigadevDiscordBot.Core.Bot.Features.Service;
using TaigadevDiscordBot.Core.Bot.Features.UserExperience;
using TaigadevDiscordBot.Core.Constants;

namespace TaigadevDiscordBot.App.Bot.Features.UserExperience
{
    public class UserLevelService : IUserLevelService
    {
                                            // lvl / exp
        private readonly ConcurrentDictionary<int, ulong> _levels = new();
        private readonly Regex _roleLevelRegex = new(@"^([0-9][0-9]?\.)|(100\.)");

        private readonly IUserRepository _userRepository;
        private readonly IRolesRepository _rolesRepository;
        private readonly DiscordSocketClient _client;
        private readonly ILogger<UserLevelService> _logger;

        private const int MaxLevel = 300;

        public UserLevelService(
            IUserRepository userRepository,
            IRolesRepository rolesRepository,
            DiscordSocketClient client, 
            ILogger<UserLevelService> logger)
        {
            _userRepository = userRepository;
            _rolesRepository = rolesRepository;
            _client = client;
            _logger = logger;

            ParallelEnumerable.Range(0, MaxLevel).ForAll(x => _levels.TryAdd(x, CalculateLevelExperience(x)));

            static ulong CalculateLevelExperience(int level) => (ulong)(level * level) * 5 + (ulong)level * 50 + 100;
        }

        public async Task LevelUpUserIfNeededAsync(ulong userId, ulong guildId)
        {
            var user = await _userRepository.GetOrCreateUserAsync(userId, guildId);

            if (user.Level == MaxLevel)
            {
                return;
            }

            while (IsRoleUpdateAvailable(user.Experience, user.Level))
            {
                user = await LevelUpUserAsync(userId, guildId);
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

        public Task<User> LevelUpUserAsync(ulong userId, ulong guildId)
        {
            return _userRepository.UpdateUserAsync(userId, guildId, async user =>
            {
                if (user.Level == MaxLevel)
                {
                    return;
                }
                
                // migration from jupiter bot activity
                await TryMigrateUserLevel(user, userId, guildId);

                // uncomment if experience should be removed after level-up
                //var requiredExperience = _levels[user.Level++];
                //user.Experience = user.Experience < requiredExperience ? 0 : user.Experience - requiredExperience;

                var guild = _client.GetGuild(guildId) as IGuild;
                var dsUser = await guild.GetUserAsync(userId);

                while (IsRoleUpdateAvailable(user.Experience, user.Level))
                {
                    user.Level++;
                }

                await UpdateRolesAsync(guild, user);

                _logger.LogInformation($"User with id '{dsUser.Id}' level updated to '{user.Level}'");
            });
        }

        private async Task TryMigrateUserLevel(User user, ulong userId, ulong guildId)
        {
            if (user.LevelMigrationNotNeeded)
            {
                return;
            }

            var guild = _client.GetGuild(guildId) as IGuild;
            var dsUser = await guild.GetUserAsync(userId);

            var userRoles = _rolesRepository.GetRoles(guildId, dsUser.RoleIds);

            var levelRoles = userRoles.Where(x => x.IsLevelRole).ToArray();
            var levelRole = levelRoles.OrderByDescending(x => x.Level).FirstOrDefault();

            user.LevelMigrationNotNeeded = true;
            if (levelRole is null || levelRole.Level == 0 || levelRole.Level == user.Level)
                return;

            user.Level = levelRole.Level!.Value;
            user.Experience = _levels[user.Level--];
        }

        public async Task<User> SetUserLevelAsync(IGuildUser dsUser, int level)
        {
            if (!_levels.TryGetValue(level, out var levelExperience))
            {
                return null;
            }

            var user = await _userRepository.UpdateUserAsync(dsUser.Id, dsUser.Guild.Id, user =>
            {
                user.Level = level;
                user.Experience = levelExperience;
                return Task.CompletedTask;
            });

            await UpdateRolesAsync(dsUser.Guild, user);
            return user;
        }

        private async Task UpdateRolesAsync(IGuild guild, User user)
        {
            var dsUser = await guild.GetUserAsync(user.UserId);
            var guildRoles = guild.Roles.ToList();
            var nextLevelRole = guildRoles.Find(x => x.Name.StartsWith($"{user.Level}."));

            // next level has no role
            if (nextLevelRole is null)
            {
                _logger.LogInformation($"Level '{user.Level}' role does not exist. User '{dsUser.Nickname ?? dsUser.Username}' leveled up anyway");
                return;
            }

            var userRoles = _rolesRepository.GetRoles(guild.Id, dsUser.RoleIds);

            var removeLevelRoleIds = userRoles
                .Where(x => x.IsLevelRole && x.Id != nextLevelRole.Id)
                .Select(x => x.Id)
                .ToArray();

            if (Env.IsDevelopmentEnvironment)
            {
                _logger.LogDebug($"User '{dsUser.Nickname ?? dsUser.Username}' role '{nextLevelRole.Name}' added. (just log, nothing happen)");
                if (removeLevelRoleIds.Length > 0)
                {
                    _logger.LogDebug($"User '{dsUser.Nickname ?? dsUser.Username}' roles removed: ['{string.Join(", ", removeLevelRoleIds)}'] removed. (just log, nothing happen)");
                }
                return;
            }

            await dsUser.AddRoleAsync(nextLevelRole.Id);
            if (removeLevelRoleIds.Length > 0)
            {
                await dsUser.RemoveRolesAsync(removeLevelRoleIds);
            }
            _logger.LogInformation($"User '{dsUser.Nickname ?? dsUser.Username}' roles updated: [{string.Join(", ", removeLevelRoleIds)}] removed, {nextLevelRole.Name} added");
        }
    }
}