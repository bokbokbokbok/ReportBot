using Hangfire.Abstractions;
using McgTgBot.DB;
using McgTgBotNet.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Hangfire.Jobs
{
    public class WorksnapsUserJob : IJob
    {
        private readonly WorksnapsService _worksnapsService;
        public static TelegramBotClient client;

        public WorksnapsUserJob()
        {
            _worksnapsService = new WorksnapsService("MCCNm0JhBxAAbhsl4CvrV3ljBVtFVrYlcGATKhFX");
            client = new TelegramBotClient("7233685875:AAGiO5CGVmL7rIMHl7t8SJLuaRTHhgL1214");
        }

        public string Id => nameof(WorksnapsUserJob);

        public async Task Run(CancellationToken cancellationToken = default)
        {
            var userFinished = await _worksnapsService.GetSummaryReportsAsync();

            foreach (var item in userFinished)
            {
                if (item.Value)
                {
                    var user = DBContext.GetUserWithProject(item.Key);

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
