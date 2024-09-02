using McgTgBotNet.Keyboards;
using McgTgBotNet.MessageHandler.Requests;
using ReportBot.Services.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Entities = McgTgBotNet.DB.Entities;

namespace McgTgBotNet.MessageHandler.Handlers
{
    public class RegisterMessageHandler : IMessageHandler
    {
        private readonly TelegramBotClient _client;
        private readonly IWorksnapsService _worksnapsService;
        private readonly IUserService _userService;

        public RegisterMessageHandler(
            TelegramBotClient client,
            IWorksnapsService worksnapsService,
            IUserService userService)
        {
            _client = client;
            _worksnapsService = worksnapsService;
            _userService = userService;
        }

        public string MessageTrigger => "/email";

        public async Task ExecuteAsync(MessageRequest request)
        {
            if (request.Update.Message.Chat.Type != ChatType.Private)
            {
                throw new InvalidOperationException("This command is only available in private chat.");
            }

            var mainKeyboard = MainKeyboard.Create();
            var message = request.Update.Message;
            var email = message!.Text;

            var worksnapsUser = await _worksnapsService.GetUserAsync(email!);
            var role = await _worksnapsService.GetUserRoleAsync(worksnapsUser.Id);
            var user = new Entities.User
            {
                ChatId = message.Chat.Id,
                WorksnapsId = worksnapsUser.Id,
                Username = worksnapsUser.Login,
                FirstName = worksnapsUser.FirstName,
                LastName = worksnapsUser.LastName!,
                Role = role.ToLower()
            };

            await _userService.AddUserAsync(user);

            await _worksnapsService.AddProjectToUser(user.WorksnapsId);

            var sent = await _client.SendTextMessageAsync(
                message.Chat.Id,
                "🎉 Thank you for registering! 🎉\n\nWe're excited to have you on board. If you have any questions, feel free to reach out!",
                replyMarkup: mainKeyboard);
        }
    }
}
