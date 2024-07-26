using System.Net;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Microsoft.Extensions.DependencyInjection;
using McgTgBotNet.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using McgTgBotNet.Extensions;

namespace McgTgBotNet;

public class Program
{
    public static TelegramBotClient client;

    public static async Task Main(string[] args)
    {
        var host = args.CreateHostBuilder().Build();
        var hostTask = host.RunAsync();

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        Console.WriteLine("Hangfire Dashboard started at: http://localhost:5000/hangfire");

        await CreateClientAsync();
        await hostTask; ;
    }

    private static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception.Message;

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }

    public static async Task<TelegramBotClient> CreateClientAsync()
    {
        var client = new TelegramBotClient(ConfigExtension.GetConfiguration("TelegramBot:Token"));

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