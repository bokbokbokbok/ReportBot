//using Microsoft.Extensions.Configuration;

//namespace McgTgBotNet.Extensions;

//public class ConfigsExtension
//{
//    public static string GetConfiguration(string item)
//    {
//        var directory = Directory.GetCurrentDirectory();

//        var builder = new ConfigurationBuilder()
//           .SetBasePath(Directory.GetCurrentDirectory())
//           .AddJsonFile("appsettings.json")
//           .AddEnvironmentVariables();

//        IConfiguration configuration = builder.Build();

//        string config = configuration[item]!;

//        if (string.IsNullOrEmpty(config))
//        {
//            Console.WriteLine("Config not found.");
//            throw new Exception("Config not found.");
//        }

//        return config;
//    }
//}

