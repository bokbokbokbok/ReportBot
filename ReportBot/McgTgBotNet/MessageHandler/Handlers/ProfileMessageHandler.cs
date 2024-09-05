using McgTgBotNet.Keyboards;
using McgTgBotNet.MessageHandler.Requests;
using ReportBot.Services.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace McgTgBotNet.MessageHandler.Handlers
{
    public class ProfileMessageHandler : IMessageHandler
    {
        private readonly TelegramBotClient _client;
        private readonly IUserService _userService;

        public ProfileMessageHandler(
            TelegramBotClient client,
            IUserService userService)
        {
            _client = client;
            _userService = userService;
        }

        public string MessageTrigger => KeyboardButtons.ProfileButton;
        public async Task ExecuteAsync(MessageRequest request)
        {
            if (request.Update.Message!.Chat.Type != ChatType.Private)
            {
                throw new InvalidOperationException("This command is only available in private chat.");
            }

            var mainKeyboard = MainKeyboard.Create();
            var message = request.Update.Message;

            var user = await _userService.GetUserByChatIdAsync(message!.Chat.Id);

            var text = $"👤 Profile\n" +
                       $"_Username:_ {user.Username}\n" +
                       $"_First Name:_ {user.FirstName}\n" +
                       $"_Last Name:_ {user.LastName}\n";
            var sent = await _client.SendTextMessageAsync(
                message.Chat.Id,
                text,
                replyMarkup: mainKeyboard,
                parseMode: ParseMode.MarkdownV2);
        }
    }
}
