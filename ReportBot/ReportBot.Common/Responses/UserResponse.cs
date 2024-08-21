using ReportBot.Common.DTOs;

namespace ReportBot.Common.Responses
{
    public class UserResponse
    {
        public UserDTO User { get; set; } = null!;
        public int TimePerDay { get; set; }
        public int TimePerWeek { get; set; }
    }
}
