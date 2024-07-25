using McgTgBotNet.Attributes;

namespace McgTgBotNet.DTOs
{
    public class TimeEntryDTO
    {
        [XMLProperty("id")]
        public int Id { get; set; }
        [XMLProperty("logged_timestamp")]
        public long LoggedTimestamp { get; set; }
        [XMLProperty("from_timestamp")]
        public long FromTimestamp { get; set; }
        [XMLProperty("project_id")]
        public int ProjectId { get; set; }
        [XMLProperty("user_id")]
        public int UserId { get; set; }
    }
}
