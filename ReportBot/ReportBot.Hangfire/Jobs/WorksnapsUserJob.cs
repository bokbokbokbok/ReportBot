using Hangfire.Abstractions;
using McgTgBotNet.DB.Entities;
using Microsoft.EntityFrameworkCore;
using ReportBot.DataBase.Repositories.Interfaces;
using ReportBot.Services.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Hangfire.Jobs
{
    public class WorksnapsUserJob : IJob
    {
        private readonly IRepository<User> _userRepository;
        private readonly IWorksnapsService _worksnapsService;
        private readonly TelegramBotClient client;

        public WorksnapsUserJob(IRepository<User> userRepository, IWorksnapsService worksnapsService)
        {
            client = new TelegramBotClient("7233685875:AAGiO5CGVmL7rIMHl7t8SJLuaRTHhgL1214");
            _userRepository = userRepository;
            _worksnapsService = worksnapsService;
        }

        public static string Id => nameof(WorksnapsUserJob);

        public async Task Run(CancellationToken cancellationToken = default)
        {
            var userFinished = await _worksnapsService.GetSummaryReportsAsync();

            foreach (var item in userFinished)
            {
                if (item.Value)
                {
                    var user = _userRepository.Include(x => x.Projects).FirstOrDefault(x => x.WorksnapsId == item.Key)
                        ?? throw new Exception("User not found");

                    List<KeyboardButton[]> buttons = new List<KeyboardButton[]>();

                    foreach (var project in user.Projects)
                    {
                        KeyboardButton[] row = new KeyboardButton[]
                        {
                            new KeyboardButton(project.Name)
                        };

                        buttons.Add(row);
                    }

                    await client.SendTextMessageAsync(
                        user.ChatId,
                        $"👋 Hello {user.FirstName} {user.LastName}!\n\nYou have successfully completed your session. Great job! 🎉\n\nNow, please select the project you are working on:",
                        replyMarkup: new ReplyKeyboardMarkup(buttons) { ResizeKeyboard = true });
                }
            }
        }
    }
}
