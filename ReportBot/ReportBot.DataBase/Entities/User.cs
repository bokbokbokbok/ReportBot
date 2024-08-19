using McgTgBotNet.Models;
using System.ComponentModel.DataAnnotations;

namespace McgTgBotNet.DB.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public long ChatId { get; set; }
        public int WorksnapsId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        public List<Report> Reports { get; set; } = new List<Report>();
        public List<Project> Projects { get; set; } = new List<Project>();
    }
}
