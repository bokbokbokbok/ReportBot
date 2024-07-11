using McgTgBot.DB;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace McgTgBotNet.Services
{
    public class MessageProcess
    {
        TelegramBotClient client;

        public MessageProcess(TelegramBotClient _client)
        {
            client = _client;
        }

        public bool ProcessMessage(Update update)
        {
            try
            {

                // Only process Message updates: https://core.telegram.org/bots/api#message
                if (update.Message != null && update.Message.Text != null)
                {
                    ProcessText(update);
                }

                if (update.CallbackQuery != null)
                    ProcessCallback(update);
            }
            catch (Exception ex)
            {
                if (update.Message != null)
                    client.SendTextMessageAsync(update.Message.Chat.Id, ex.Message);
            }
            return true;
        }

        public void ProcessUpdate(Update update)
        {

        }

        public void ProcessText(Update update)
        {
            var message = update.Message;

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

            if (message.Text.ToLower().Contains("help"))
            {
                var buttons = new KeyboardButton[][]
                {
                    new KeyboardButton[] { "Select menu" },
                    new KeyboardButton[] { "Daylireport format" }, new KeyboardButton[] { "Close" }
                };

                var sent = client.SendTextMessageAsync(message.Chat.Id, "Choose a response",
                    replyMarkup: new ReplyKeyboardMarkup(buttons) { ResizeKeyboard = true });
            }

            if (message.Text.ToLower().Contains("daylireport format"))
            {
                var text = "#dailyreport - маркер для бота\r\n#nickname - тг ник работника\r\n#projectname - название проекта\r\n#date - дата смены\r\n#time - затраченное время\r\n";

                var sent = client.SendTextMessageAsync(message.Chat.Id, text);
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
                string first;
                using (var reader = new StringReader(message.Text))
                {
                    first = reader.ReadLine();
                }
                first = Regex.Replace(first, @"\s+", "");

                var values = first.Split('#');
                values = values.Where(p => p.Length > 0).ToArray();

                if (values.Length > 1)
                {
                    var reportUser = DBContext.GetReportsUser((int)message.From.Id);
                    var nickname = values[1];

                    if (reportUser == null)
                    {
                        reportUser = DBContext.CreateUser(message, nickname);

                        client.SendTextMessageAsync(message.Chat.Id, "Hello " +reportUser.UserName+ "! Nice to meet you.");
                    }
                    else if (reportUser.UserName != nickname)
                    {
                        var oldName = reportUser.UserName;
                        reportUser.UserName = nickname;
                        DBContext.UpdateUser(reportUser);

                        client.SendTextMessageAsync(message.Chat.Id, "What a nice new name you got here! Changed " + oldName + " for " + reportUser.UserName);
                    }
                }


                DBContext.CreateReport(message);

                client.SendTextMessageAsync(message.Chat.Id, "Thank you for your report");
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

                var dbUsers = DBContext.GetReportsUsers().Select(p => p.UserName);
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

                var reports = DBContext.GetReports(DateTime.MinValue, DateTime.MaxValue);

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
    }
}
