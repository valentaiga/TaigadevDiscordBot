using System.Threading.Tasks;

using Discord.WebSocket;

using Microsoft.Extensions.Logging;

using TaigadevDiscordBot.Core.Bot.Event.EventArgs;
using TaigadevDiscordBot.Core.Bot.Features;
using TaigadevDiscordBot.Core.Bot.Features.Service;

namespace TaigadevDiscordBot.App.Bot.Features.Service
{
    public class RolesService : IRolesService
    {
        private readonly IUserRepository _userRepository;
        private readonly IGuildRepository _guildRepository;
        private readonly IRolesRepository _rolesRepository;
        private readonly ILogger<RolesService> _logger;

        public RolesService(
            IUserRepository userRepository, 
            IGuildRepository guildRepository,
            IRolesRepository rolesRepository,
            ILogger<RolesService> logger)
        {
            _userRepository = userRepository;
            _guildRepository = guildRepository;
            _rolesRepository = rolesRepository;
            _logger = logger;
        }

        public async ValueTask SetRolesToNewUser(UserJoinedEventArgs eventArgs)
        {
            var user = await _userRepository.GetOrCreateUserAsync(eventArgs.User.Id, eventArgs.User.Guild.Id);

            if (user.Roles.Count == 0)
            {
                await SetInitialRoles(eventArgs.User);
            }
            else
            {
                await RestoreRoles(eventArgs.User, user);
            }
        }

        private async Task SetInitialRoles(SocketGuildUser dsUser)
        {
            var guild = await _guildRepository.GetGuildAsync(dsUser.Guild.Id);

            if (guild.DefaultRoles.Count == 0)
            {
                return;
            }

            await dsUser.AddRolesAsync(guild.DefaultRoles);
            _logger.LogDebug($"Added initial roles to user '{dsUser.Nickname ?? dsUser.Username}' with id '{dsUser.Id}'");
        }

        private async Task RestoreRoles(SocketGuildUser dsUser, User user)
        {
            await dsUser.AddRolesAsync(user.Roles);
            _logger.LogDebug($"Restored roles to user '{dsUser.Nickname ?? dsUser.Username}' with id '{dsUser.Id}'");
        }
    }
}