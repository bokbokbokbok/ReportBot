using McgTgBotNet.MessageHandler.Requests;

namespace McgTgBotNet.MessageHandler.Handlers
{
    public interface IMessageHandler
    {
        public string MessageTrigger { get; }

        public Task ExecuteAsync(MessageRequest request);
    }
}
