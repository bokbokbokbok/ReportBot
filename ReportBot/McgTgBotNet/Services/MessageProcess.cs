using McgTgBotNet.DB.Entities;
using McgTgBotNet.Extensions;
using McgTgBotNet.Keyboards;
using McgTgBotNet.Models;
using McgTgBotNet.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReportBot.Common.DTOs;
using ReportBot.DataBase.Repositories.Interfaces;
using ReportBot.Services.Services.Interfaces;
using System.Globalization;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace McgTgBotNet.Services
{
    public class MessageProcess : IMessageProcess
    {
        private readonly TelegramBotClient client;
        private readonly IWorksnapsService _worksnapsService;
        private readonly IUserService _userService;
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<DB.Entities.User> _userRepository;
        private readonly IReportService _reportService;
        private static Dictionary<string, Message> messageHistory = new Dictionary<string, Message>();

        public MessageProcess(
            IWorksnapsService worksnapsService,
            IUserService userService,
            IRepository<Project> projectRepository,
            IReportService reportService,
            IRepository<DB.Entities.User> userRepository)
        {
            client = new TelegramBotClient(ConfigExtension.GetConfiguration("TelegramBot:Token"));
            _worksnapsService = worksnapsService;
            _userService = userService;
            _projectRepository = projectRepository;
            _reportService = reportService;
            _userRepository = userRepository;
        }

        public async Task<bool> ProcessMessageAsync(Update update)
        {
            try
            {
                // Only process Message updates: https://core.telegram.org/bots/api#message
                if (update.Message != null && update.Message.Text != null)
                {
                    await ProcessTextAsync(update);
                }
            }
            catch (Exception ex)
            {
                if (update.Message != null)
                    await client.SendTextMessageAsync(update.Message.Chat.Id, ex.Message, replyMarkup: MainKeyboard.Create());
            }
            return true;
        }

        public async Task ProcessTextAsync(Update update)
        {
            var message = update.Message;
            var mainKeyboard = MainKeyboard.Create();
            Console.WriteLine($"Received a text message {message!.Chat.Type}.");

            if (message!.Text!.ToLower().Contains("start"))
            {
                if (message.Chat.Type == ChatType.Group ||
                    message.Chat.Type == ChatType.Supergroup)
                {
                    var projects = await _projectRepository.ToListAsync();
                    List<KeyboardButton[]> buttons = new List<KeyboardButton[]>();

                    foreach (var project in projects)
                    {
                        KeyboardButton[] row = new KeyboardButton[]
                        {
                            new KeyboardButton(project.Name)
                        };

                        buttons.Add(row);
                    }

                    var sent = await client.SendTextMessageAsync(
                            message.Chat.Id,
                            "👋 Hi there! To add this chat to your project, please select a project. Thank you!",
                            replyMarkup: new ReplyKeyboardMarkup(buttons) { ResizeKeyboard = true });
                }
                else
                {
                    var user = await _userRepository.FirstOrDefaultAsync(x => x.ChatId == message.Chat.Id);
                    if (user == null)
                    {
                        var sent = await client.SendTextMessageAsync(
                            message.Chat.Id,
                            "👋Hi! Please enter your Worksnaps email.\r\n\r\nThank you!",
                            replyMarkup: mainKeyboard);
                    }
                    else
                    {
                        var sent = await client.SendTextMessageAsync(
                             message.Chat.Id,
                             $"👋Hi, {user.FirstName} {user.LastName}! How can I help you?",
                             replyMarkup: mainKeyboard);
                    }
                }
            }

            if (await IsProjectExistAsync(message.Text.ToLower()) && (message.Chat.Type == ChatType.Group || message.Chat.Type == ChatType.Supergroup))
            {
                var project = await _projectRepository.FirstOrDefaultAsync(x => x.Name.ToLower() == message.Text.ToLower())
                    ?? throw new Exception("Project not found");

                project.GroupId = message.Chat.Id;

                await _projectRepository.UpdateAsync(project);

                var sent = await client.SendTextMessageAsync(
                            message.Chat.Id,
                            "🎉 Congratulations! The chat has been successfully added to the project.",
                            replyMarkup: new ReplyKeyboardRemove());
            }

            if (message.Text.ToLower().Contains("profile"))
            {
                var user = await _userService.GetUserByChatIdAsync(message.Chat.Id);

                var text = $"👤 Profile\n" +
                           $"_Username:_ {user.Username}\n" +
                           $"_First Name:_ {user.FirstName}\n" +
                           $"_Last Name:_ {user.LastName}\n";
                var sent = await client.SendTextMessageAsync(
                    message.Chat.Id,
                    text,
                    replyMarkup: mainKeyboard,
                    parseMode: ParseMode.MarkdownV2);
            }

            if (Regex.IsMatch(message.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase))
            {
                var email = message.Text;

                var worksnapsUser = await _worksnapsService.GetUserAsync(email);
                var role = await _worksnapsService.GetUserRoleAsync(worksnapsUser.Id);
                var user = new DB.Entities.User
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

                var sent = await client.SendTextMessageAsync(
                    message.Chat.Id,
                    "🎉 Thank you for registering! 🎉\n\nWe're excited to have you on board. If you have any questions, feel free to reach out!",
                    replyMarkup: mainKeyboard);
            }

            if (message.Text.ToLower().Contains("my reports"))
            {
                var reports = await _reportService.GetReportsForUserAsync(message.Chat.Id);

                var text = "📋 Your reports:\n\n";

                foreach (var report in reports)
                {
                    text += $"💻 Project: {report.Project.Name}\n" +
                            $"📅 Date: {report.DateOfShift.Date}\n" +
                            $"⏰ Time: {report.TimeOfShift} minutes\n" +
                            $"📝 {report.Message}\n\n";
                }

                var sent = await client.SendTextMessageAsync(
                    message.Chat.Id,
                    text,
                    replyMarkup: mainKeyboard);
            }

            if (message.Text.ToLower().Contains("add daylireport"))
            {
                var user = _userRepository.Include(x => x.Projects).FirstOrDefault(x => x.ChatId == message.Chat.Id)
                        ?? throw new Exception("User not found");

                List<KeyboardButton[]> buttons = new List<KeyboardButton[]>();

                foreach (var project in user.Projects)
                {
                    KeyboardButton[] row = new KeyboardButton[]
                    {
                            new KeyboardButton(project.Name)
                    };

                    buttons.Add(row);
                }

                await client.SendTextMessageAsync(
                    user.ChatId,
                    $"👋 Hello {user.FirstName} {user.LastName}!\n\nNow, please select the project you are working on:",
                    replyMarkup: new ReplyKeyboardMarkup(buttons) { ResizeKeyboard = true });
            }

            if (await IsProjectExistAsync(message.Text.ToLower()) && message.Chat.Type == ChatType.Private)
            {
                messageHistory.Add("project", message);

                var keyboard = new List<KeyboardButton[]>
                {
                    new[]
                    {
                        new KeyboardButton(DateTime.Today.AddDays(-2).ToString("dd/MM/yyyy")),
                        new KeyboardButton(DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy")),
                        new KeyboardButton(DateTime.Today.ToString("dd/MM/yyyy")),
                    }
                };

                var send = await client.SendTextMessageAsync(
                    message.Chat.Id,
                    "📅 Please select your shift date. If your date isn’t listed, please input it manually (format 23/07/2024):",
                    replyMarkup: new ReplyKeyboardMarkup(keyboard) { ResizeKeyboard = true });
            }

            if (IsDate(message.Text))
            {
                messageHistory.Add("date", message);

                var text = "Please, describe what you did. Example:\n\n#time: 300\r\nПрацював над проектами. Зробив щоб для конкретного юзера проекти підтягувались з worksnaps. Також зробив зв'язки між проектами та юзерами.\n\n#time - wasted time\r\n";

                var send = await client.SendTextMessageAsync(
                    message.Chat.Id,
                    text,
                    replyMarkup: new ReplyKeyboardRemove());
            }

            if (message.Text.ToLower() == "close")
            {
                var sent = await client.SendTextMessageAsync(message.Chat.Id, "Removing keyboard",
                    replyMarkup: new ReplyKeyboardRemove());
            }

            if (message.Text.ToLower().Contains("#time"))
            {
                string pattern = @"#time:\s*(?<time>\d+)";

                Match match = Regex.Match(message.Text, pattern, RegexOptions.IgnoreCase);

                var time = int.Parse(match.Groups["time"].Value);

                var dateMessage = messageHistory["date"];
                var projectName = messageHistory["project"];

                if (dateMessage.Text == null)
                    throw new Exception("Sorry, but you have not entered a date");

                if (projectName.Text == null)
                    throw new Exception("Sorry, but you have not entered a project");

                DateTime date = DateTime.ParseExact(dateMessage.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                var report = new CreateReportDTO
                {
                    ChatId = message.Chat.Id,
                    DateOfShift = date,
                    TimeOfShift = time,
                    Created = DateTime.Now,
                    Message = message.Text,
                    UserName = message.From!.Username!,
                    ProjectName = projectName.Text,
                };

                var result = await _reportService.AddReportAsync(report);

                messageHistory.Clear();

                await client.SendTextMessageAsync(message.Chat.Id, "Thank you for your report", replyMarkup: mainKeyboard);
            }
        }

        private async Task<bool> IsProjectExistAsync(string projectName)
        {
            var project = await _projectRepository.FirstOrDefaultAsync(x => x.Name.ToLower() == projectName.ToLower());

            if (project == null)
                return false;

            return true;
        }

        public static bool IsDate(string text)
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

            return isValidDate;
        }
    }
}
