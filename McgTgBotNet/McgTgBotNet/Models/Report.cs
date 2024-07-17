using McgTgBotNet.DB.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace McgTgBotNet.Models
{
    public partial class Report
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public string Message { get; set; }
        public string UserName { get; set; }
        public int ChatId { get; set; }
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        [ForeignKey(nameof(Project))]
        public int ProjectId { get; set; }

        public User User { get; set; }
        public Project Project { get; set; }
    }
}
