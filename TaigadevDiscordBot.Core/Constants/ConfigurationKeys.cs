namespace TaigadevDiscordBot.Core.Constants
{
    public static class ConfigurationKeys
    {
        public static class Discord
        {
            public const string AdminId = "Discord:AdminId";
            public const string Token = "Discord:Token";
            public const string Prefix = "Discord:Prefix";
            public const string WorkServerIds = "Discord:WorkServerIds";
            public const string ServiceCategoryName = "Discord:ServiceCategoryName";
            public const string ServiceChannels = "Discord:ServiceChannels";
        }

        public static class Redis
        {
            public const string Endpoint = "Redis:Endpoint";
            public const string Password = "Redis:Password";
            public const string Prefix = "Redis:Prefix";
        }
    }
}