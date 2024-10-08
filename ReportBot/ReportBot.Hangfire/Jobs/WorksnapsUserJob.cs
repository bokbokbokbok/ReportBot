﻿using Hangfire.Abstractions;
using McgTgBotNet.DB.Entities;
using McgTgBotNet.Extensions;
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
            client = new TelegramBotClient(ConfigExtension.GetConfiguration("TelegramBot:Token"));
            _userRepository = userRepository;
            _worksnapsService = worksnapsService;
        }

        public static string Id => nameof(WorksnapsUserJob);

        public async Task Run(CancellationToken cancellationToken = default)
        {
            var userFinished = await _worksnapsService.GetFinishedReportsAsync();

            foreach (var item in userFinished)
            {
                var user = _userRepository.Include(x => x.Projects).FirstOrDefault(x => x.WorksnapsId == item.UserId)
                    ?? throw new Exception("User not found");

                var buttons = new List<KeyboardButton[]>();

                var markup = new InlineKeyboardMarkup(
                    user.Projects.Select(project => new List<InlineKeyboardButton>() { InlineKeyboardButton.WithCallbackData(project.Name, $"#project-{project.Name}") }));

                await client.SendTextMessageAsync(
                    user.ChatId,
                    $"👋 Hello {user.FirstName} {user.LastName}!\n\nYou have successfully completed your session. Great job! 🎉\n\nNow, please select the project you are working on:",
                    replyMarkup: markup);
            }
        }
    }
}