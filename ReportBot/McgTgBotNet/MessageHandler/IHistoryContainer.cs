namespace McgTgBotNet.MessageHandler
{
    public interface IHistoryContainer
    {
        void Clear();
        string? Pull(string key);
        void Push(string key, string value);
    }
}
