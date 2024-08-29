using McgTgBotNet.MessageHandler.Requests;

namespace McgTgBotNet.Services.Interfaces
{
    public interface IMessageProcessor
    {
        Task Process(MessageRequest request);
    }
}
