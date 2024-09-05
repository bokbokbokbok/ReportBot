using McgTgBotNet.Keyboards;
using McgTgBotNet.MessageHandler.Requests;
using ReportBot.Common.DTOs;
using ReportBot.Services.Services.Interfaces;
using System.Globalization;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace McgTgBotNet.MessageHandler.Handlers
{
    public class DailyReportMessageHandler : IMessageHandler
    {
        private readonly TelegramBotClient _client;
        private readonly IReportService _reportService;
        private readonly IHistoryContainer _historyContainer;

        public DailyReportMessageHandler(
            IHistoryContainer historyContainer,
            TelegramBotClient client,
            IReportService reportService)
        {
            _historyContainer = historyContainer;
            _client = client;
            _reportService = reportService;
        }

        public string MessageTrigger => "/dailyreport";

        public async Task ExecuteAsync(MessageRequest request)
        {
            if (request.Update.Message!.Chat.Type != ChatType.Private)
            {
                throw new InvalidOperationException("This command is only available in private chat.");
            }

            var mainKeyboard = MainKeyboard.Create();
            var message = request.Update.Message;

            var dateMessage = _historyContainer.Pull("date");
            var projectName = _historyContainer.Pull("project");

            if (dateMessage == null)
                throw new Exception("Sorry, but you have not entered a date");

            if (projectName == null)
                throw new Exception("Sorry, but you have not entered a project");

            DateTime date = DateTime.ParseExact(dateMessage, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            _historyContainer.Clear();

            var report = new CreateReportDTO
            {
                ChatId = message!.Chat.Id,
                DateOfShift = date,
                Created = DateTime.Now,
                Message = message.Text!,
                UserName = message.From!.Username!,
                ProjectName = projectName,
            };

            var result = await _reportService.AddReportAsync(report);


            await _client.SendTextMessageAsync(message.Chat.Id, "Thank you for your report", replyMarkup: mainKeyboard);
        }
    }
}
