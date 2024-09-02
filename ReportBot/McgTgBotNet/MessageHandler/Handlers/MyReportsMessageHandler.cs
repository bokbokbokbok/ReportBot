using McgTgBotNet.Keyboards;
using McgTgBotNet.MessageHandler.Requests;
using ReportBot.Services.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace McgTgBotNet.MessageHandler.Handlers
{
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
            if (request.Update.Message.Chat.Type != ChatType.Private)
            {
                throw new InvalidOperationException("This command is only available in private chat.");
            }

            var mainKeyboard = MainKeyboard.Create();
            var message = request.Update.Message;

            var reports = await _reportService.GetReportsForUserAsync(message.Chat.Id);

            var text = "📋 Your reports:\n\n";

            foreach (var report in reports)
            {
                text += $"💻 Project: {report.Project.Name}\n" +
                        $"📅 Date: {report.DateOfShift.Date:dd MMM yyyy}\n" +
                        $"📝 {report.Message}\n\n";
            }

            var sent = await _client.SendTextMessageAsync(
                message.Chat.Id,
                text,
                replyMarkup: mainKeyboard);
        }
    }
}
