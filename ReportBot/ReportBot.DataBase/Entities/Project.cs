using McgTgBotNet.DB.Entities;
using System.ComponentModel.DataAnnotations;

namespace McgTgBotNet.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }
        public int WorksnapsId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public long? GroupId { get; set; }

        public List<Report> Reports { get; set; } = new List<Report>();
        public List<User> Users { get; set; } = new List<User>();
    }
}
