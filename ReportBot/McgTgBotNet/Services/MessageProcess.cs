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
using Telegram.Bot.Types.ReplyMarkups;

namespace McgTgBotNet.Services
{
    public class MessageProcess : IMessageProcess
    {
        TelegramBotClient client;
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
            client = new TelegramBotClient("7233685875:AAGiO5CGVmL7rIMHl7t8SJLuaRTHhgL1214");
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

                if (update.CallbackQuery != null)
                    ProcessCallback(update);
            }
            catch (Exception ex)
            {
                var buttons = new KeyboardButton[][]
                {
                    new KeyboardButton[] { "Profile" },
                    new KeyboardButton[] { "Update shift time" },
                    new KeyboardButton[] { "Add daylireport" },
                    new KeyboardButton[] { "My reports" },
                    new KeyboardButton[] { "Close" }
                };

                if (update.Message != null)
                    await client.SendTextMessageAsync(update.Message.Chat.Id, ex.Message, replyMarkup: new ReplyKeyboardMarkup(buttons) { ResizeKeyboard = true });
            }
            return true;
        }

        public void ProcessUpdate(Update update)
        {

        }

        public async Task ProcessTextAsync(Update update)
        {
            var message = update.Message;

            if (message.Text.ToLower().Contains("start"))
            {
                var buttons = new KeyboardButton[][]
                {
                    new KeyboardButton[] { "Profile" },
                    new KeyboardButton[] { "Update shift time" },
                    new KeyboardButton[] { "Add daylireport" },
                    new KeyboardButton[] { "My reports" },
                    new KeyboardButton[] { "Close" }
                };

                var sent = client.SendTextMessageAsync(
                    message.Chat.Id,
                    "👋Hi! Please enter your Worksnaps email and the shift time in minutes.\r\n\r\nExample:\r\n/email: example@example.com\r\n/shiftTime: 120\r\n\r\nThank you!",
                    replyMarkup: new ReplyKeyboardMarkup(buttons) { ResizeKeyboard = true });
            }

            if (message.Text.ToLower().Contains("/email") && message.Text.ToLower().Contains("/shifttime"))
            {
                string pattern = @"/email:\s*(?<email>[^ \r\n]+)\s*/shiftTime:\s*(?<shiftTime>\d+)";

                Match match = Regex.Match(message.Text, pattern, RegexOptions.IgnoreCase);

                var id = await _worksnapsService.GetUserId(match.Groups["email"].Value);
                int shiftTime = int.Parse(match.Groups["shiftTime"].Value);
                var user = new DB.Entities.User
                { 
                    ChatId = message.Chat.Id,
                    WorksnapsId = id,
                    Username = message.From.Username,
                    FirstName = message.From.FirstName,
                    LastName = message.From.LastName,
                    ShiftTime = shiftTime
                };

                await _userService.AddUserAsync(user);

                await _worksnapsService.AddProjectToUser(user.WorksnapsId);
                var buttons = new KeyboardButton[][]
                {
                    new KeyboardButton[] { "Select menu" },
                    new KeyboardButton[] { "Update shift time" },
                    new KeyboardButton[] { "Add daylireport" },
                    new KeyboardButton[] { "Close" }
                };

                var sent = client.SendTextMessageAsync(
                    message.Chat.Id,
                    "🎉 Thank you for registering! 🎉\n\nWe're excited to have you on board. If you have any questions, feel free to reach out!",
                    replyMarkup: new ReplyKeyboardMarkup(buttons) { ResizeKeyboard = true }
                    );
            }

            if (message.Text.ToLower().Contains("update shift time"))
            {
                var sent = client.SendTextMessageAsync(
                    message.Chat.Id,
                    "Please enter your new shift time in minutes.\n\nExample:\n/updateShiftTime: 120"
                    );
            }

            if (message.Text.ToLower().Contains("/updateshifttime"))
            {
                var shiftTime = int.Parse(Regex.Match(message.Text, @"\d+").Value);

                await _userService.UpdateUserShiftTimeAsync(message.Chat.Id, shiftTime);

                var sent = client.SendTextMessageAsync(
                    message.Chat.Id,
                    "Thank you for updating shift time"
                    );
            }

            if (message.Text.ToLower().Contains("select menu"))
            {
                var buttons = new InlineKeyboardButton[][]
                {
                    new[] // first row
                    {
                        InlineKeyboardButton.WithCallbackData("dailyreports", "SelectUserStep"),
                        InlineKeyboardButton.WithCallbackData("empty", "12"),
                    },
                    new[] // second row
                    {
                        InlineKeyboardButton.WithCallbackData("empty", "21"),
                        InlineKeyboardButton.WithCallbackData("empty", "22"), 
                    },
                };

                var sent = client.SendTextMessageAsync(message.Chat.Id, "Please select an option",
                    replyMarkup: new InlineKeyboardMarkup(buttons));

            }

            //if (message.Text.ToLower().Contains("help"))
            //{
            //    var buttons = new KeyboardButton[][]
            //    {
            //        new KeyboardButton[] { "Select menu" },
            //        new KeyboardButton[] { "Update shift time" }, new KeyboardButton[] { "Daylireport format" }, new KeyboardButton[] { "Close" }
            //    };

            //    var sent = client.SendTextMessageAsync(message.Chat.Id, "Choose a response",
            //        replyMarkup: new ReplyKeyboardMarkup(buttons) { ResizeKeyboard = true });
            //}

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

            if (IsProjectExist(message.Text.ToLower()))
            {
                messageHistory.Add(message);

                var text = "Please, describe what you did. Example:\n\n#dailyreport #date: 07/23/2024 #time: 300\r\nПрацював над проектами. Зробив щоб для конкретного юзера проекти підтягувались з worksnaps. Також зробив зв'язки між проектами та юзерами.\n\n#dailyreport - маркер для бота\r\n#date - дата смены\r\n#time - затраченное время\r\n";

                var send = client.SendTextMessageAsync(message.Chat.Id, text,
                    replyMarkup: new ReplyKeyboardRemove());
            }

            if (message.Text.ToLower() == "close")
            {
                var sent = client.SendTextMessageAsync(message.Chat.Id, "Removing keyboard",
                    replyMarkup: new ReplyKeyboardRemove());
            }


            if (message.Text.ToLower().Contains("testcallback"))
            {
                var buttons = new InlineKeyboardButton[][]
                {
                    new[] // first row
                    {
                        InlineKeyboardButton.WithCallbackData("1.1", "11"),
                        InlineKeyboardButton.WithCallbackData("1.2", "12"),
                    },
                    new[] // second row
                    {
                        InlineKeyboardButton.WithCallbackData("2.1", "21"),
                        InlineKeyboardButton.WithCallbackData("2.2", "22"),
                    },
                };

                var sent = client.SendTextMessageAsync(message.Chat.Id, "",
                    replyMarkup: new InlineKeyboardMarkup(buttons));
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

                await client.SendTextMessageAsync(message.Chat.Id, "Thank you for your report");
            }

        }

        public void ProcessCallback(Update update)
        {
            var message = update.CallbackQuery;

            var text = message.Data ?? "";

            if (text == "SelectUserStep")
            {
                var users = new List<string>();
                users.Add("TestUser1");
                users.Add("TestUser21");
                users.Add("TestUser31");
                users.Add("TestUser41");
                users.Add("TestUser51");
                users.Add("TestUser61");
                users.Add("TestUser71");

                var dbUsers = new List<string>();
                users.AddRange(dbUsers);

                var buttons = new List<List<InlineKeyboardButton>>();
                var row = new List<InlineKeyboardButton>();

                foreach (var user in users)
                {
                    row.Add(InlineKeyboardButton.WithCallbackData(user, "SelectProjectStep=" + user));

                    if (row.Count == 2)
                    {
                        buttons.Add(row);
                        row = new List<InlineKeyboardButton>();
                    }
                }

                if (row.Count > 0)
                    buttons.Add(row);

                var obj = new { Title = "dailyreports Selection" };
                var objJson = JsonConvert.SerializeObject(obj);

                var sent = client.EditMessageTextAsync(message.Message.Chat.Id, message.Message.MessageId, objJson,
                    replyMarkup: new InlineKeyboardMarkup(buttons));
            }

            if (text.StartsWith("SelectProjectStep="))
            {

                var selected = text.Replace("SelectProjectStep=", "");

                var projects = new List<string>();
                projects.Add("TestProject1");
                projects.Add("TestProject21");
                projects.Add("TestProject31");
                projects.Add("TestProject41");
                projects.Add("TestProject51");
                projects.Add("TestProject61");
                projects.Add("TestProject71");

                var buttons = new List<List<InlineKeyboardButton>>();
                var row = new List<InlineKeyboardButton>();

                foreach (var project in projects)
                {
                    row.Add(InlineKeyboardButton.WithCallbackData(project, "SelectDateStep=" + project));

                    if (row.Count == 2)
                    {
                        buttons.Add(row);
                        row = new List<InlineKeyboardButton>();
                    }
                }

                if (row.Count > 0)
                    buttons.Add(row);

                row = new List<InlineKeyboardButton>();
                row.Add(InlineKeyboardButton.WithCallbackData("Reset", "SelectUserStep"));

                var obj = new { Title = "dailyreports Selection", SelectedUser = selected };
                var objJson = JsonConvert.SerializeObject(obj);

                var sent = client.EditMessageTextAsync(message.Message.Chat.Id, message.Message.MessageId, objJson,
                    replyMarkup: new InlineKeyboardMarkup(buttons));
            }

            if (text.StartsWith("SelectDateStep="))
            {

                var selected = text.Replace("SelectDateStep=", "");

                var dates = new List<string>();
                dates.Add("Today");
                dates.Add("Yesterday");
                dates.Add("Last week");
                dates.Add("Specify date");

                var buttons = new List<List<InlineKeyboardButton>>();
                var row = new List<InlineKeyboardButton>();

                foreach (var date in dates)
                {
                    row.Add(InlineKeyboardButton.WithCallbackData(date, "SelectedReportsStep=" + date));

                    if (row.Count == 2)
                    {
                        buttons.Add(row);
                        row = new List<InlineKeyboardButton>();
                    }
                }

                if (row.Count > 0)
                    buttons.Add(row);

                var currObjJSON = message.Message.Text;
                JObject currObj = JObject.Parse(currObjJSON);
                currObj.Add("SelectedProject", selected);

                var objJson = JsonConvert.SerializeObject(currObj);

                var sent = client.EditMessageTextAsync(message.Message.Chat.Id, message.Message.MessageId, objJson,
                    replyMarkup: new InlineKeyboardMarkup(buttons));
            }

            if (text.StartsWith("SelectedReportsStep="))
            {

                var selected = text.Replace("SelectDateStep=", "");

                var reports = new List<Report>();

                var msgText = "Here is your result:";
                msgText += "\r\n";

                foreach (var report in reports)
                {
                    msgText += "User: " + report.UserName;
                    msgText += "\r\n";
                    msgText += "Date: " + report.Created.ToString();
                    msgText += "\r\n";
                    msgText += "Report message:";
                    msgText += "\r\n";
                    msgText += report.Message;
                    msgText += "\r\n";
                    msgText += "\r\n";
                }

                var sent = client.EditMessageTextAsync(message.Message.Chat.Id, message.Message.MessageId, msgText);
            }
        }

        private bool IsProjectExist(string projectName)
        {
            var project = _projectRepository.FirstOrDefault(x => x.Name.ToLower() == projectName.ToLower());

            if (project == null)
                return false;

            return true;
        }
    }
}
