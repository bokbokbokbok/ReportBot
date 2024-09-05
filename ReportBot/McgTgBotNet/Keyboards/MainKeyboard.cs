using Telegram.Bot.Types.ReplyMarkups;

namespace McgTgBotNet.Keyboards
{
    public class MainKeyboard
    {
        public static IReplyMarkup Create()
        {
            var profile = new KeyboardButton(KeyboardButtons.ProfileButton);
            var myReports = new KeyboardButton(KeyboardButtons.ReportsButton);
            var addReport = new KeyboardButton(KeyboardButtons.AddReportButton);
            var close = new KeyboardButton(KeyboardButtons.CloseButton);

            var buttons = new List<KeyboardButton[]>
            {
                new KeyboardButton[] { profile, myReports },
                new KeyboardButton[] { addReport },
                new KeyboardButton[] { close }
            };

            var keyboard = new ReplyKeyboardMarkup(buttons)
            {
                ResizeKeyboard = true
            };

            return keyboard;
        }

        public static IReplyMarkup CreateForGroup()
        {
            var createManagerReport = new KeyboardButton(KeyboardButtons.CreateManagerReport);
            var addProjectToChat = new KeyboardButton(KeyboardButtons.AddProjectToChat);

            var buttons = new List<KeyboardButton[]>
            {
                new KeyboardButton[] { addProjectToChat, createManagerReport }
            };

            var keyboard = new ReplyKeyboardMarkup(buttons)
            {
                ResizeKeyboard = true
            };

            return keyboard;
        }
    }
}
