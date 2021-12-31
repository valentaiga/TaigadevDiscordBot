namespace TaigadevDiscordBot.Core.Database.Redis
{
    public interface IRedisEntity
    {
        string GetCacheKey();
    }
}