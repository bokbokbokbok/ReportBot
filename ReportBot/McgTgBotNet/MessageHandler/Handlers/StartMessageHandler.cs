﻿using McgTgBotNet.Keyboards;
using McgTgBotNet.MessageHandler.Requests;
using Microsoft.EntityFrameworkCore;
using ReportBot.DataBase.Entities;
using ReportBot.DataBase.Repositories.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Entities = McgTgBotNet.DB.Entities;

namespace McgTgBotNet.MessageHandler.Handlers
{
    public class StartMessageHandler : IMessageHandler
    {
        private readonly TelegramBotClient _client;
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<Entities.User> _userRepository;

        public string MessageTrigger => "/start";

        public StartMessageHandler(
            TelegramBotClient client,
            IRepository<Project> projectRepository,
            IRepository<Entities.User> userRepository)
        {
            _projectRepository = projectRepository;
            _client = client;
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(MessageRequest request)
        {
            var mainKeyboard = MainKeyboard.Create();
            var message = request.Update.Message;

            if (message.Chat.Type == ChatType.Group ||
                message.Chat.Type == ChatType.Supergroup)
            {
                var projects = await _projectRepository.ToListAsync();
                var markup = new InlineKeyboardMarkup(
                        projects.Select(project => new List<InlineKeyboardButton>() { InlineKeyboardButton.WithCallbackData(project.Name, $"#project-{project.Name}") }));

                var sent = await _client.SendTextMessageAsync(
                        message.Chat.Id,
                        $"👋Hi! How can I help you?",
                        replyMarkup: MainKeyboard.CreateForGroup());
            }
            else
            {
                var user = await _userRepository.FirstOrDefaultAsync(x => x.ChatId == message.Chat.Id);
                if (user == null)
                {
                    var sent = await _client.SendTextMessageAsync(
                        message.Chat.Id,
                        "👋Hi! Please enter your Worksnaps email.\r\n\r\nThank you!",
                        replyMarkup: mainKeyboard);
                }
                else
                {
                    var sent = await _client.SendTextMessageAsync(
                         message.Chat.Id,
                         $"👋Hi, {user.FirstName} {user.LastName}! How can I help you?",
                         replyMarkup: mainKeyboard);
                }
            }
        }
    }
}
