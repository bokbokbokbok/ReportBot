using Hangfire;
using McgTgBotNet.Hangfire.Extensions;
using System.Net;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using McgTgBotNet.Extensions;
using Microsoft.Extensions.DependencyInjection;
using McgTgBotNet.Services.Interfaces;

namespace McgTgBotNet;

public class Program
{
    public static TelegramBotClient client;

    public static async Task Main(string[] args)
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        await CreateClientAsync();

        Console.ReadLine();
    }

    private static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception.Message;

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }

    public static async Task<TelegramBotClient> CreateClientAsync()
    {
        var token = "7233685875:AAGiO5CGVmL7rIMHl7t8SJLuaRTHhgL1214";
        var client = new TelegramBotClient(token);

        CancellationTokenSource cts = new CancellationTokenSource();
        ReceiverOptions receiverOptions = new ReceiverOptions()
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };
        var me = await client.GetMeAsync();
        Console.WriteLine(
          $"Hello, World! I am user {me.Id} and my name is {me.FirstName}."
        );
        client.StartReceiving(
            updateHandler: BotOnMessageReceived,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );

        return client;
    }

    private static async Task BotOnMessageReceived(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.ConfigureServices();
        var serviceProvider = serviceCollection.BuildServiceProvider();

        using (var scope = serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider.GetRequiredService<IMessageProcess>();
            await scopedService.ProcessMessageAsync(update);
        }
    }
}