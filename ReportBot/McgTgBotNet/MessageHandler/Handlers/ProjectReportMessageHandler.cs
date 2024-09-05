using McgTgBotNet.Keyboards;
using McgTgBotNet.MessageHandler.Requests;
using Microsoft.EntityFrameworkCore;
using ReportBot.DataBase.Entities;
using ReportBot.DataBase.Repositories.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace McgTgBotNet.MessageHandler.Handlers
{
    public class ProjectReportMessageHandler : IMessageHandler
    {
        private readonly IRepository<Project> _projectRepository;
        private readonly IHistoryContainer _historyContainer;
        private readonly TelegramBotClient _client;

        public ProjectReportMessageHandler(
            IRepository<Project> projectRepository,
            TelegramBotClient client,
            IHistoryContainer historyContainer)
        {
            _projectRepository = projectRepository;
            _client = client;
            _historyContainer = historyContainer;
        }

        public string MessageTrigger => "#project-";

        public async Task ExecuteAsync(MessageRequest request)
        {
            var projectName = request.Update.CallbackQuery!.Data!.Replace(MessageTrigger, "");

            var isProjectExists = await IsProjectExistAsync(projectName);

            if (!isProjectExists)
            {
                throw new InvalidOperationException($"Unable to find project: {projectName}");
            }

            var chat = request.Update.CallbackQuery.Message!.Chat;

            if (chat.Type is ChatType.Group or ChatType.Supergroup)
            {
                await GroupProjectAsync(projectName, chat.Id);
            }
            else if (chat.Type is ChatType.Private)
            {
                await PrivateProjectAsync(chat.Id, projectName);
            }
        }

        private async Task PrivateProjectAsync(long chatId, string data)
        {
            _historyContainer.Push("project", data);

            var keyboard = new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData($"Today ({DateTime.Today:dd/MM/yyyy})", $"#date-{DateTime.Today:dd/MM/yyyy}") },
                new[] { InlineKeyboardButton.WithCallbackData($"Yesterday ({DateTime.Today.AddDays(-1):dd/MM/yyyy})", $"#date-{DateTime.Today.AddDays(-1):dd/MM/yyyy}") },
                new[] { InlineKeyboardButton.WithCallbackData($"Before Yesterday ({DateTime.Today.AddDays(-2):dd/MM/yyyy})", $"#date-{DateTime.Today.AddDays(-2):dd/MM/yyyy}") }
            }.ToList();

            var send = await _client.SendTextMessageAsync(
                chatId,
                $"📅 Please select your shift date. If your date isn’t listed, please input it manually (format {DateTime.Now:dd/MM/yyyy}):",
                replyMarkup: new InlineKeyboardMarkup(keyboard));
        }

        private async Task GroupProjectAsync(string projectName, long chatId)
        {
            var project = await _projectRepository.FirstOrDefaultAsync(x => x.Name.ToLower().Equals(projectName.ToLower())) ?? throw new Exception("Project not found");

            project.GroupId = chatId;

            await _projectRepository.UpdateAsync(project);

            var sent = await _client.SendTextMessageAsync(
                        chatId,
                        "🎉 Congratulations! The chat has been successfully added to the project.",
                        replyMarkup: MainKeyboard.CreateForGroup());
        }

        private async Task<bool> IsProjectExistAsync(string projectName)
        {
            var project = await _projectRepository.FirstOrDefaultAsync(x => x.Name.ToLower().Equals(projectName.ToLower()));

            return project != null;
        }
    }
}
