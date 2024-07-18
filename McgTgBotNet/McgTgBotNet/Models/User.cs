using McgTgBotNet.Models;
using System.Collections.Generic;
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
        public int ShiftTime { get; set; }

        public List<Report> Reports { get; set; }
    }
}
