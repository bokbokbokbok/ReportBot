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
        var host = args.CreateHostBuilder().Build();

        host.SetupHangfire();

        using (var server = new BackgroundJobServer())
        {
            Console.WriteLine("Hangfire Server started.");

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var token = "7233685875:AAGiO5CGVmL7rIMHl7t8SJLuaRTHhgL1214";

            client = new TelegramBotClient(token);

            CancellationTokenSource cts = new CancellationTokenSource();
            ReceiverOptions receiverOptions = new ReceiverOptions()
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
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


            //client.StartReceiving();
            //while(true){}
            Console.ReadLine();
        //client.StopReceiving();
        }
    }

    private static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception.Message;

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }

    public TelegramBotClient CreateClient()
    {
        var token = "";

        var client = new TelegramBotClient(token);
        //client.OnMessage += BotOnMessageReceived;
        //client.OnMessageEdited += BotOnMessageReceived;
        //client.StartReceiving();
        CancellationTokenSource cts = new CancellationTokenSource();
        ReceiverOptions receiverOptions = new ReceiverOptions()
        {
            AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
        };
        var me = client.GetMeAsync().Result;
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

        // Create a scope manually
        using (var scope = serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider.GetRequiredService<IMessageProcess>();
            await scopedService.ProcessMessageAsync(update);
        }
    }
}