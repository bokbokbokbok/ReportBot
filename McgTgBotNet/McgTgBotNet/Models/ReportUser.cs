using System;

namespace McgTgBotNet.Models
{
    public partial class ReportUser
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public string UserName { get; set; }
        public int UserId { get; set; }
    }
}
