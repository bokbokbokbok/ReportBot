using McgTgBotNet.Keyboards;
using McgTgBotNet.MessageHandler.Requests;
using Microsoft.EntityFrameworkCore;
using ReportBot.DataBase.Entities;
using ReportBot.DataBase.Repositories.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Entities = McgTgBotNet.DB.Entities;

namespace McgTgBotNet.MessageHandler.Handlers
{
    public class AddProjectToChatHandler : IMessageHandler
    {
        private readonly TelegramBotClient _client;
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<Entities.User> _userRepository;

        public string MessageTrigger => KeyboardButtons.AddProjectToChat;

        public AddProjectToChatHandler(
            TelegramBotClient client,
            IRepository<Project> projectRepository,
            IRepository<Entities.User> userRepository)
        {
            _projectRepository = projectRepository;
            _client = client;
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(MessageRequest request)
        {
            var mainKeyboard = MainKeyboard.Create();
            var message = request.Update.Message;

            if (message.Chat.Type == ChatType.Group ||
                message.Chat.Type == ChatType.Supergroup)
            {
                var projects = await _projectRepository.ToListAsync();
                var markup = new InlineKeyboardMarkup(
                        projects.Select(project => new List<InlineKeyboardButton>() { InlineKeyboardButton.WithCallbackData(project.Name, $"#project-{project.Name}") }));

                var sent = await _client.SendTextMessageAsync(
                        message.Chat.Id,
                        "👋 Hi there! To add this chat to your project, please select a project. Thank you!",
                        replyMarkup: markup);
            }
        }
    }
}
