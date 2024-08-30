using McgTgBotNet.Keyboards;
using McgTgBotNet.MessageHandler;
using McgTgBotNet.MessageHandler.Handlers;
using McgTgBotNet.MessageHandler.Requests;
using McgTgBotNet.Services.Interfaces;
using ReportBot.Common;
using System.Globalization;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace McgTgBotNet.Services
{
    public class MessageProcessor : IMessageProcessor
    {
        private readonly TelegramBotClient _client;
        private readonly IHistoryContainer _historyContainer;
        private readonly IEnumerable<IMessageHandler> _handlers;

        public MessageProcessor(
            IEnumerable<IMessageHandler> handlers,
            IHistoryContainer historyContainer,
            TelegramBotClient client)
        {
            _handlers = handlers;
            _historyContainer = historyContainer;
            _client = client;
        }

        public async Task Process(MessageRequest request)
        {
            try
            {
                await ExecuteHandlerAsync(request);
            }
            catch (Exception ex)
            {
                _historyContainer.Clear();

                long? chatId = request.Type switch
                {
                    UpdateType.Message => request.Update.Message.Chat.Id,
                    UpdateType.CallbackQuery => request.Update.CallbackQuery.Message.Chat.Id,
                    _ => null,
                };

                if (chatId != null)
                {
                    await _client.SendTextMessageAsync(chatId, ex.Message, replyMarkup: MainKeyboard.Create());
                }
            }
        }

        private async Task ExecuteHandlerAsync(MessageRequest request)
        {
            Func<IMessageHandler, bool> predicate;

            if (request.Type == UpdateType.Message)
            {
                var text = (request.Update?.Message?.Text) ?? throw new InvalidOperationException("Failed to retrieve message text.The message is null.");

                predicate = (h) => h.MessageTrigger == text
                    || text.StartsWith(h.MessageTrigger)
                    || h.MessageTrigger == IsDate(text)
                    || h.MessageTrigger == _historyContainer.Pull("dailyreport")
                    || h.MessageTrigger == IsEmail(text);
            }
            else if (request.Type == UpdateType.CallbackQuery)
            {
                var text = request.Update.CallbackQuery.Data ?? throw new InvalidOperationException("Failed to retrieve message text.The message is null.");

                predicate = (h) => text.StartsWith(h.MessageTrigger);
            }
            else
            {
                throw new NotImplementedException();
            }

            var handler = _handlers.FirstOrDefault(predicate) ?? throw new InvalidDataException($"Unable to find handler for specified command: {request.Update}");

            await handler.ExecuteAsync(request);
        }

        private string IsDate(string text)
        {
            string format = "dd/MM/yyyy";

            DateTime parsedDate;
            bool isValidDate = DateTime.TryParseExact(
                text,
                format,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out parsedDate
            );

            return isValidDate ? "#date-" : "";
        }
        private string IsEmail(string text)
        {
            var isEmail = Regex.IsMatch(text, Regexes.EmailRegex, RegexOptions.IgnoreCase);

            return isEmail ? "/email" : "";
        }
    }
}
