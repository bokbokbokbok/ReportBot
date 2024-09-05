using McgTgBotNet.MessageHandler.Requests;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace McgTgBotNet.MessageHandler.Handlers
{
    public class DateSelectMessageHandler : IMessageHandler
    {
        private readonly TelegramBotClient _client;
        private readonly IHistoryContainer _historyContainer;


        public DateSelectMessageHandler(
            TelegramBotClient client,
            IHistoryContainer historyContainer)
        {
            _client = client;
            _historyContainer = historyContainer;
        }

        public string MessageTrigger => "#date-";

        public async Task ExecuteAsync(MessageRequest request)
        {
            var text = "Please, describe what you did. Example:\n\n<i>Працював над проектами. Зробив щоб для конкретного юзера проекти підтягувались з worksnaps. Також зробив зв'язки між проектами та юзерами.</i>";

            if (request.Type == UpdateType.Message)
            {
                _historyContainer.Push("date", request.Update.Message!.Text!);
                _historyContainer.Push("dailyreport", "/dailyreport");
                var send = await _client.SendTextMessageAsync(
                    request.Update.Message!.Chat.Id,
                    text,
                    replyMarkup: new ReplyKeyboardRemove(),
                    parseMode: ParseMode.Html);
            }
            else if (request.Type == UpdateType.CallbackQuery)
            {
                _historyContainer.Push("date", request.Update.CallbackQuery.Data.Replace(MessageTrigger, ""));
                _historyContainer.Push("dailyreport", "/dailyreport");

                var send = await _client.SendTextMessageAsync(
                    request.Update.CallbackQuery.Message!.Chat,
                    text,
                    replyMarkup: new ReplyKeyboardRemove(),
                    parseMode: ParseMode.Html);
            }
        }
    }
}
