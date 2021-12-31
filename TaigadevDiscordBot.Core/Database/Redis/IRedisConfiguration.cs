namespace TaigadevDiscordBot.Core.Database.Redis
{
    public interface IRedisConfiguration
    {
        string Endpoint { get; }

        string Password { get; }

        string Prefix { get; }
    }
}