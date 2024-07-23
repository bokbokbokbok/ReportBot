using System;
using System.ComponentModel.DataAnnotations;

namespace McgTgBotNet.Models
{
    public partial class ReportUser
    {
        [Key]
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int UserId { get; set; }
    }
}
