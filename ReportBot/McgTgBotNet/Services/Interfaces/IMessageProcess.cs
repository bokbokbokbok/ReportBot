using Telegram.Bot.Types;

namespace McgTgBotNet.Services.Interfaces;

public interface IMessageProcess
{
    Task<bool> ProcessMessageAsync(Update update);
}
