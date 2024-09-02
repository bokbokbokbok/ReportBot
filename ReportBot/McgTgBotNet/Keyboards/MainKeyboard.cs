using Telegram.Bot.Types.ReplyMarkups;

namespace McgTgBotNet.Keyboards
{
    public class MainKeyboard
    {
        public static IReplyMarkup Create()
        {
            KeyboardButton profile = new KeyboardButton(KeyboardButtons.ProfileButton);
            KeyboardButton myReports = new KeyboardButton(KeyboardButtons.ReportsButton);
            KeyboardButton addReport = new KeyboardButton(KeyboardButtons.AddReportButton);
            KeyboardButton close = new KeyboardButton(KeyboardButtons.CloseButton);

            List<KeyboardButton[]> buttons = new List<KeyboardButton[]>
            {
                new KeyboardButton[] { profile, myReports },
                new KeyboardButton[] { addReport },
                new KeyboardButton[] { close }
            };

            ReplyKeyboardMarkup keyboard = new ReplyKeyboardMarkup(buttons)
            {
                ResizeKeyboard = true
            };

            return keyboard;
        }

        public static IReplyMarkup CreateForGroup()
        {
            KeyboardButton createManagerReport = new KeyboardButton(KeyboardButtons.CreateManagerReport);

            List<KeyboardButton[]> buttons = new List<KeyboardButton[]>
            {
                new KeyboardButton[] { createManagerReport }
            };

            ReplyKeyboardMarkup keyboard = new ReplyKeyboardMarkup(buttons)
            {
                ResizeKeyboard = true
            };

            return keyboard;
        }
    }
}
