using System;

namespace McgTgBotNet.Models
{
    public partial class Report
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public string Message { get; set; }
        public string UserName { get; set; }
        public int ChatId { get; set; }
        public int UserId { get; set; }
    }
}
