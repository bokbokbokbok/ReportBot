using McgTgBotNet.Keyboards;
using McgTgBotNet.MessageHandler.Requests;
using Microsoft.EntityFrameworkCore;
using ReportBot.DataBase.Repositories.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Entities = McgTgBotNet.DB.Entities;

namespace McgTgBotNet.MessageHandler.Handlers
{
    public class AddDailyReportMessageHandler : IMessageHandler
    {
        private readonly TelegramBotClient _client;
        private readonly IRepository<Entities.User> _userRepository;

        public AddDailyReportMessageHandler(
            TelegramBotClient client,
            IRepository<Entities.User> userRepository)
        {
            _client = client;
            _userRepository = userRepository;
        }

        public string MessageTrigger => KeyboardButtons.AddReportButton;

        public async Task ExecuteAsync(MessageRequest request)
        {
            if (request.Update.Message!.Chat.Type != ChatType.Private)
            {
                throw new InvalidOperationException("This command is only available in private chat.");
            }

            var user = _userRepository.Include(x => x.Projects).FirstOrDefault(x => x.ChatId == request.Update.Message.Chat.Id)
                        ?? throw new Exception("User not found");

            var markup = new InlineKeyboardMarkup(
                user.Projects.Select(project => new List<InlineKeyboardButton>() { InlineKeyboardButton.WithCallbackData(project.Name, $"#project-{project.Name}") }));

            await _client.SendTextMessageAsync(
                user.ChatId,
                $"👋 Hello {user.FirstName} {user.LastName}!\n\nNow, please select the project you are working on:",
                replyMarkup: markup);
        }
    }
}
