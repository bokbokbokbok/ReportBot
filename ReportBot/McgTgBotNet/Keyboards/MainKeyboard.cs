using McgTgBotNet.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace McgTgBotNet.Keyboards
{
    public class MainKeyboard
    {
        public static IReplyMarkup Create()
        {
            KeyboardButton profile = new KeyboardButton("👤 Profile");
            KeyboardButton myReports = new KeyboardButton("📋 My Reports");
            KeyboardButton updateShiftTime = new KeyboardButton("⏱ Update Shift Time");
            KeyboardButton addReport = new KeyboardButton("📝 Add daylireport");
            KeyboardButton close = new KeyboardButton("Close");

            KeyboardButton[][] buttons = new KeyboardButton[][]
            {
                new KeyboardButton[] { profile, myReports},
                new KeyboardButton[] { updateShiftTime, addReport},
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
