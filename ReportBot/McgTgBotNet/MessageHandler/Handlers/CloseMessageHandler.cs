using McgTgBotNet.Keyboards;
using McgTgBotNet.MessageHandler.Requests;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace McgTgBotNet.MessageHandler.Handlers
{
    public class CloseMessageHandler : IMessageHandler
    {
        private readonly TelegramBotClient _client;
        public CloseMessageHandler(TelegramBotClient client)
        {
            _client = client;
        }

        public string MessageTrigger => KeyboardButtons.CloseButton;

        public async Task ExecuteAsync(MessageRequest request)
        {
            if (request.Update.Message.Chat.Type != ChatType.Private)
            {
                throw new InvalidOperationException("This command is only available in private chat.");
            }

            var sent = await _client.SendTextMessageAsync(request.Update.Message!.Chat.Id, "Removing keyboard",
                    replyMarkup: new ReplyKeyboardRemove());
        }
    }
}
