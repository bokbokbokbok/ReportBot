using Telegram.Bot.Types;

namespace McgTgBotNet.MessageHandler
{
    public class HistoryContainer : IHistoryContainer
    {
        private static readonly Dictionary<string, string> _history = new();

        public void Push(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new InvalidDataException($"Unable to push history object with empty key. Value: {value}");
            }

            if (value == null)
            {
                throw new InvalidDataException($"Unable to push empty object to history. Key: {key}");
            }

            if (_history.TryGetValue(key, out _))
            {
                throw new InvalidOperationException($"Unable to push key to history. There is already value with same key.");
            }

            _history.Add(key, value);
        }

        public string? Pull(string key)
        {
            if (!_history.TryGetValue(key, out var result))
            {
                return null;
            }

            return result;
        }

        public void Clear()
        {
            _history.Clear();
        }
    }
}
