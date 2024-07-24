using McgTgBotNet.DB.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace McgTgBotNet.Models
{
    public partial class Report
    {
        [Key]
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime DateOfShift { get; set; }
        public int TimeOfShift { get; set; }
        public string UserName { get; set; } = string.Empty;
        public long ChatId { get; set; }
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        [ForeignKey(nameof(Project))]
        public int ProjectId { get; set; }

        public User User { get; set; } = null!;
        public Project Project { get; set; } = null!;
    }
}
