namespace TaigadevDiscordBot.App.Constants
{
    public static class ConfigurationKeys
    {
        public static class Discord
        {
            public const string AdminId = "Discord:AdminId";
            public const string Token = "Discord:Token";
        }

        public static class Redis
        {
            public const string Endpoint = "Redis:Endpoint";
            public const string Password = "Redis:Password";
            public const string Prefix = "Redis:Prefix";
        }
    }
}