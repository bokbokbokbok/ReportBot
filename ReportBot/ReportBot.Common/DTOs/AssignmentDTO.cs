using McgTgBotNet.Attributes;

namespace McgTgBotNet.DTOs
{
    public class AssignmentDTO
    {
        [XMLProperty("id")]
        public int Id { get; set; }
        [XMLProperty("user_id")]
        public int UserId { get; set; }
        [XMLProperty("project_id")]
        public int ProjectId { get; set; }
        [XMLProperty("role")]
        public string Role { get; set; } = string.Empty;
    }
}
