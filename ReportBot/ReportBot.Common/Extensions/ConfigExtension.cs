using Microsoft.Extensions.Configuration;

namespace McgTgBotNet.Extensions;

public static class ConfigExtension
{
    public static string GetConfiguration(string item)
    {
        var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (directory != null && !directory.GetFiles("*.sln").Any())
        {
            directory = directory.Parent;
        }

        if (directory == null)
        {
            Console.WriteLine("Solution directory not found.");
            throw new Exception("Solution directory not found.");
        }

        var builder = new ConfigurationBuilder()
                .SetBasePath(directory.ToString())
                .AddJsonFile("McgTgBotNet\\appsettings.json")
                .AddEnvironmentVariables();


        IConfiguration configuration = builder.Build();

        string config = configuration[item]!;

        if (string.IsNullOrEmpty(config))
        {
            Console.WriteLine("Config not found.");
            throw new Exception("Config not found.");
        }

        return config;
    }
}

