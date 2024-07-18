﻿using Hangfire;
using McgTgBotNet.Hangfire.Extensions;
using McgTgBotNet.Services;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace McgTgBotNet
{
    public class Program
    {
        public static TelegramBotClient client;
        private static string[] svins = new string[] { "Managers" };
        private static bool isSvin(string svin) => svins.Contains(svin);

        public static async Task Main(string[] args)
        {
            var host = HangfireExtensions.CreateHostBuilder(args).Build();

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

        public static TelegramBotClient CreateClient()
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
            var processor = new MessageProcess(client);

            await processor.ProcessMessageAsync(update);
        }
    }
}