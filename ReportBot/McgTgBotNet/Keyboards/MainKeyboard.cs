using Telegram.Bot.Types.ReplyMarkups;

namespace McgTgBotNet.Keyboards
{
    public class MainKeyboard
    {
        public static IReplyMarkup Create()
        {
            KeyboardButton profile = new KeyboardButton("👤 Profile");
            KeyboardButton myReports = new KeyboardButton("📋 My Reports");
            KeyboardButton addReport = new KeyboardButton("📝 Add daylireport");
            KeyboardButton close = new KeyboardButton("Close");

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
    }
}
