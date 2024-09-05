using McgTgBotNet.Keyboards;
using McgTgBotNet.MessageHandler.Requests;
using ReportBot.Services.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace McgTgBotNet.MessageHandler.Handlers
{
    public class CreateManagerReportHandler : IMessageHandler
    {
        private readonly TelegramBotClient _client;
        private readonly IReportService _reportService;

        public CreateManagerReportHandler(
            TelegramBotClient client,
            IReportService reportService)
        {
            _client = client;
            _reportService = reportService;
        }

        public string MessageTrigger => KeyboardButtons.CreateManagerReport;

        public async Task ExecuteAsync(MessageRequest request)
        {
            if (request.Update.Message!.Chat.Type != ChatType.Group)
            {
                throw new InvalidOperationException("This command is only available in group.");
            }

            var managerReport = await _reportService.GetManagerReportAsync();

            var text = "";

            foreach (var report in managerReport)
            {
                text += $"<b>{report.ProjectName}: {report.TotalTime} minutes</b>\n" +
                        $"{string.Join("\n", report.Messages)}\n\n";
            }

            var sent = await _client.SendTextMessageAsync(
                        request.Update.Message.Chat.Id,
                        text,
                        replyMarkup: MainKeyboard.CreateForGroup(),
                        parseMode: ParseMode.Html);
        }
    }
}
