using System;

namespace TaigadevDiscordBot.Core.Constants
{
    public static class Env
    {
        public static string EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        public static bool IsDevelopmentEnvironment = string.Equals(
            EnvironmentName,
            "Development",
            StringComparison.OrdinalIgnoreCase);
    }
}