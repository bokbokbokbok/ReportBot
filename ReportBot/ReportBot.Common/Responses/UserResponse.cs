using McgTgBotNet.DTOs;

namespace ReportBot.Common.Responses
{
    public class UserResponse
    {
        public WorksnapsUserDTO User { get; set; } = null!;
        public int TimePerDay { get; set; }
        public int TimePerWeek { get; set; }
    }
}
