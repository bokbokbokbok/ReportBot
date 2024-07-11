using McgTgBot.DB;
using McgTgBotNet.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace McgTgBotNet
{
    public class Program
    {
        public static TelegramBotClient client;
        private static string[] svins = new string[] { "Managers" };
        private static bool isSvin(string svin) => svins.Contains(svin);

        public static void Main(string[] args)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var token = "6976626679:AAGbQaOPbuAcpAyRBlVUR5PGS5nnh6ChnDQ";

            client = new TelegramBotClient(token);

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

            //client.StartReceiving();
            //while(true){}
            Console.ReadLine();
            //client.StopReceiving();
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

            processor.ProcessMessage(update);
        }
    }
}
