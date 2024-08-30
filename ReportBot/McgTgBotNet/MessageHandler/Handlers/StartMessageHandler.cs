using McgTgBotNet.Keyboards;
using McgTgBotNet.MessageHandler.Requests;
using Microsoft.EntityFrameworkCore;
using ReportBot.Common.DTOs;
using ReportBot.DataBase.Entities;
using ReportBot.DataBase.Repositories.Interfaces;
using ReportBot.Services.Services.Interfaces;
using System.Globalization;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Entities = McgTgBotNet.DB.Entities;

namespace McgTgBotNet.MessageHandler.Handlers
{
    public class StartMessageHandler : IMessageHandler
    {
        private readonly TelegramBotClient _client;
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<Entities.User> _userRepository;

        public string MessageTrigger => "/start";

        public StartMessageHandler(
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
            else
            {
                var user = await _userRepository.FirstOrDefaultAsync(x => x.ChatId == message.Chat.Id);
                if (user == null)
                {
                    var sent = await _client.SendTextMessageAsync(
                        message.Chat.Id,
                        "👋Hi! Please enter your Worksnaps email.\r\n\r\nThank you!",
                        replyMarkup: mainKeyboard);
                }
                else
                {
                    var sent = await _client.SendTextMessageAsync(
                         message.Chat.Id,
                         $"👋Hi, {user.FirstName} {user.LastName}! How can I help you?",
                         replyMarkup: mainKeyboard);
                }
            }
        }
    }

    public class ProjectReportMessageHandler : IMessageHandler
    {
        private readonly IRepository<Project> _projectRepository;
        private readonly IHistoryContainer _historyContainer;
        private readonly TelegramBotClient _client;

        public ProjectReportMessageHandler(
            IRepository<Project> projectRepository,
            TelegramBotClient client,
            IHistoryContainer historyContainer)
        {
            _projectRepository = projectRepository;
            _client = client;
            _historyContainer = historyContainer;
        }

        public string MessageTrigger => "#project-";

        public async Task ExecuteAsync(MessageRequest request)
        {
            var projectName = request.Update.CallbackQuery.Data.Replace(MessageTrigger, "");

            var isProjectExists = await IsProjectExistAsync(projectName);

            if (!isProjectExists)
            {
                throw new InvalidOperationException($"Unable to find project: {projectName}");
            }

            var chat = request.Update.CallbackQuery.Message!.Chat;

            if (chat.Type is ChatType.Group or ChatType.Supergroup)
            {
                await GroupProjectAsync(projectName, chat.Id);
            }
            else if (chat.Type is ChatType.Private)
            {
                await PrivateProjectAsync(chat.Id, projectName);
            }
        }

        private async Task PrivateProjectAsync(long chatId, string data)
        {
            _historyContainer.Push("project", data);

            List<InlineKeyboardButton[]> keyboard = new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData($"Today ({DateTime.Today:dd/MM/yyyy})", $"#date-{DateTime.Today:dd/MM/yyyy}") },
                new[] { InlineKeyboardButton.WithCallbackData($"Yesterday ({DateTime.Today.AddDays(-1):dd/MM/yyyy})", $"#date-{DateTime.Today.AddDays(-1):dd/MM/yyyy}") },
                new[] { InlineKeyboardButton.WithCallbackData($"Before Yesterday ({DateTime.Today.AddDays(-2):dd/MM/yyyy})", $"#date-{DateTime.Today.AddDays(-2):dd/MM/yyyy}") }
            }.ToList();

            var send = await _client.SendTextMessageAsync(
                chatId,
                $"📅 Please select your shift date. If your date isn’t listed, please input it manually (format {DateTime.Now:dd/MM/yyyy}):",
                replyMarkup: new InlineKeyboardMarkup(keyboard));
        }

        private async Task GroupProjectAsync(string projectName, long chatId)
        {
            var project = await _projectRepository.FirstOrDefaultAsync(x => x.Name.ToLower().Equals(projectName.ToLower())) ?? throw new Exception("Project not found");

            project.GroupId = chatId;

            await _projectRepository.UpdateAsync(project);

            var sent = await _client.SendTextMessageAsync(
                        chatId,
                        "🎉 Congratulations! The chat has been successfully added to the project.",
                        replyMarkup: new ReplyKeyboardRemove());
        }

        private async Task<bool> IsProjectExistAsync(string projectName)
        {
            var project = await _projectRepository.FirstOrDefaultAsync(x => x.Name.ToLower().Equals(projectName.ToLower()));

            return project != null;
        }

    }

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

    public class DailyReportMessageHandler : IMessageHandler
    {
        private readonly TelegramBotClient _client;
        private readonly IReportService _reportService;
        private readonly IHistoryContainer _historyContainer;

        public DailyReportMessageHandler(
            IHistoryContainer historyContainer,
            TelegramBotClient client,
            IReportService reportService)
        {
            _historyContainer = historyContainer;
            _client = client;
            _reportService = reportService;
        }

        public string MessageTrigger => "/dailyreport";

        public async Task ExecuteAsync(MessageRequest request)
        {
            var mainKeyboard = MainKeyboard.Create();
            var message = request.Update.Message;

            var dateMessage = _historyContainer.Pull("date");
            var projectName = _historyContainer.Pull("project");

            if (dateMessage == null)
                throw new Exception("Sorry, but you have not entered a date");

            if (projectName == null)
                throw new Exception("Sorry, but you have not entered a project");

            DateTime date = DateTime.ParseExact(dateMessage, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            _historyContainer.Clear();

            var report = new CreateReportDTO
            {
                ChatId = message!.Chat.Id,
                DateOfShift = date,
                Created = DateTime.Now,
                Message = message.Text!,
                UserName = message.From!.Username!,
                ProjectName = projectName,
            };

            var result = await _reportService.AddReportAsync(report);


            await _client.SendTextMessageAsync(message.Chat.Id, "Thank you for your report", replyMarkup: mainKeyboard);
        }
    }

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

    public class MyReportsMessageHandler : IMessageHandler
    {
        private readonly TelegramBotClient _client;
        private readonly IReportService _reportService;

        public MyReportsMessageHandler(TelegramBotClient client, IReportService reportService)
        {
            _client = client;
            _reportService = reportService;
        }

        public string MessageTrigger => KeyboardButtons.ReportsButton;

        public async Task ExecuteAsync(MessageRequest request)
        {
            var mainKeyboard = MainKeyboard.Create();
            var message = request.Update.Message;

            var reports = await _reportService.GetReportsForUserAsync(message.Chat.Id);

            var text = "📋 Your reports:\n\n";

            foreach (var report in reports)
            {
                text += $"💻 Project: {report.Project.Name}\n" +
                        $"📅 Date: {report.DateOfShift.Date}\n" +
                        $"📝 {report.Message}\n\n";
            }

            var sent = await _client.SendTextMessageAsync(
                message.Chat.Id,
                text,
                replyMarkup: mainKeyboard);
        }
    }

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
            var sent = await _client.SendTextMessageAsync(request.Update.Message!.Chat.Id, "Removing keyboard",
                    replyMarkup: new ReplyKeyboardRemove());
        }
    }
}
