using Microsoft.Extensions.Configuration;

namespace McgTgBotNet.Extensions;

public static class ConfigExtension
{
    public static string GetConfiguration(string item)
    {
        var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .AddEnvironmentVariables();

        var configuration = builder.Build();

        var config = configuration[item]!;

        if (string.IsNullOrEmpty(config))
        {
            Console.WriteLine("Config not found.");
            throw new Exception("Config not found.");
        }

        return config;
    }
}

