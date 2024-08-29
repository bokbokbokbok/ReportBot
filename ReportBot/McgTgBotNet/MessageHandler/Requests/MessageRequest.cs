using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace McgTgBotNet.MessageHandler.Requests
{
    public class MessageRequest
    {
        public UpdateType Type { get; set; }

        public Update Update { get; set; }
    }
}
