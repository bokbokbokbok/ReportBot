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
        private static List<Message> messageHistory = new List<Message>();

        public MessageProcess(
            IWorksnapsService worksnapsService,
            IUserService userService,
            IRepository<Project> projectRepository,
            IReportService reportService,
            IRepository<DB.Entities.User> userRepository)
        {
            client = new TelegramBotClient(ConfigsExtension.GetConfiguration("TelegramBot:Token"));
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
            var userChat = await _userRepository.FirstOrDefaultAsync(x => x.ChatId == message!.Chat.Id);
            var mainKeyboard = MainKeyboard.Create();
            if (userChat != null)
                mainKeyboard = MainKeyboard.Create(userChat);

            if (message!.Text!.ToLower().Contains("start"))
            {
                var user = await _userRepository.FirstOrDefaultAsync(x => x.ChatId == message.Chat.Id);

                if (user == null)
                {
                    var sent = await client.SendTextMessageAsync(
                        message.Chat.Id,
                        "👋Hi! Please enter your Worksnaps email and the shift time in minutes.\r\n\r\nExample:\r\n/email: example@example.com\r\n/shiftTime: 120\r\n\r\nThank you!",
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

            if (message!.Text!.ToLower().Contains("manager dashboard"))
            {
                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithUrl("Visit dashboard", "https://www.youtube.com/")
                    }
                });

                await client.SendTextMessageAsync(message.Chat.Id, "📊 The Manager Dashboard is a dedicated panel designed for managers to efficiently manage and work with reports.", replyMarkup: inlineKeyboard);
            }

            if (message.Text.ToLower().Contains("profile"))
            {
                var user = await _userService.GetUserByChatIdAsync(message.Chat.Id);

                var text = $"👤 Profile\n" +
                           $"_Username:_ {user.Username}\n" +
                           $"_First Name:_ {user.FirstName}\n" +
                           $"_Last Name:_ {user.LastName}\n" +
                           $"_Shift Time:_ {user.ShiftTime} minutes";

                var sent = await client.SendTextMessageAsync(
                    message.Chat.Id,
                    text,
                    replyMarkup: mainKeyboard,
                    parseMode: ParseMode.MarkdownV2);
            }

            if (message.Text.ToLower().Contains("/email") && message.Text.ToLower().Contains("/shifttime"))
            {
                string pattern = @"/email:\s*(?<email>[^ \r\n]+)\s*/shiftTime:\s*(?<shiftTime>\d+)";

                Match match = Regex.Match(message.Text, pattern, RegexOptions.IgnoreCase);

                var worksnapsUser = await _worksnapsService.GetUserAsync(match.Groups["email"].Value);
                var role = await _worksnapsService.GetUserRoleAsync(worksnapsUser.Id);
                int shiftTime = int.Parse(match.Groups["shiftTime"].Value);
                var user = new DB.Entities.User
                {
                    ChatId = message.Chat.Id,
                    WorksnapsId = worksnapsUser.Id,
                    Username = worksnapsUser.Login,
                    FirstName = worksnapsUser.FirstName,
                    LastName = worksnapsUser.LastName!,
                    ShiftTime = shiftTime,
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

            if (message.Text.ToLower().Contains("update shift time"))
            {
                var sent = await client.SendTextMessageAsync(
                    message.Chat.Id,
                    "Please enter your new shift time in minutes.\n\nExample:\n/updateShiftTime: 120"
                    );
            }

            if (message.Text.ToLower().Contains("/updateshifttime"))
            {
                var shiftTime = int.Parse(Regex.Match(message.Text, @"\d+").Value);

                await _userService.UpdateUserShiftTimeAsync(message.Chat.Id, shiftTime);

                var sent = await client.SendTextMessageAsync(
                    message.Chat.Id,
                    "Thank you for updating shift time"
                    );
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

            if (await IsProjectExistAsync(message.Text.ToLower()))
            {
                messageHistory.Add(message);

                var text = "Please, describe what you did. Example:\n\n#dailyreport #date: 07/23/2024 #time: 300\r\nПрацював над проектами. Зробив щоб для конкретного юзера проекти підтягувались з worksnaps. Також зробив зв'язки між проектами та юзерами.\n\n#dailyreport - маркер для бота\r\n#date - дата смены\r\n#time - затраченное время\r\n";

                var send = await client.SendTextMessageAsync(message.Chat.Id, text,
                    replyMarkup: new ReplyKeyboardRemove());
            }

            if (message.Text.ToLower() == "close")
            {
                var sent = await client.SendTextMessageAsync(message.Chat.Id, "Removing keyboard",
                    replyMarkup: new ReplyKeyboardRemove());
            }

            if (message.Text.ToLower().Contains("#dailyreport"))
            {
                string pattern = @"#date:\s*(?<date>[^ ]+)\s*#time:\s*(?<time>\d+)";

                Match match = Regex.Match(message.Text, pattern, RegexOptions.IgnoreCase);

                var date = DateTime.Parse(match.Groups["date"].Value);
                var time = int.Parse(match.Groups["time"].Value);

                var projectName = messageHistory.Last().Text
                    ?? throw new Exception("Sorry, but you have not entered a project");

                var report = new CreateReportDTO
                {
                    ChatId = message.Chat.Id,
                    DateOfShift = date,
                    TimeOfShift = time,
                    Created = DateTime.Now,
                    Message = message.Text,
                    UserName = message.From!.Username!,
                    ProjectName = projectName,
                };

                var result = await _reportService.AddReportAsync(report);

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
    }
}
