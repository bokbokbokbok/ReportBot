using Hangfire.Abstractions;
using McgTgBot.DB;
using McgTgBotNet.Services;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Hangfire.Jobs
{
    public class WorksnapsUserJob : IJob
    {
        private readonly WorksnapsService _worksnapsService;
        public static TelegramBotClient client;

        public WorksnapsUserJob()
        {
            _worksnapsService = new WorksnapsService("mEJbcCmiAMBc95Fsf3FaOO22ElEdc1YJ78vkK4z7");
            client = new TelegramBotClient("7233685875:AAGiO5CGVmL7rIMHl7t8SJLuaRTHhgL1214");
        }

        public string Id => nameof(WorksnapsUserJob);

        public async Task Run(CancellationToken cancellationToken = default)
        {
            var userFinished = await _worksnapsService.GetTimeEntryAsync();

            foreach (var item in userFinished)
            {
                if (item.Value)
                {
                    var user = DBContext.GetUserByWorksnapsId(item.Key);
                    await client.SendTextMessageAsync(user.ChatId, $"Пользователь {item.Key} завершил сессию");
                }
            }
        }
    }
}
