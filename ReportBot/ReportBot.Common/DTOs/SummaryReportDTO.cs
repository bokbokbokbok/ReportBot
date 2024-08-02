using McgTgBotNet.Attributes;
using System;

namespace McgTgBotNet.DTOs
{
    public class SummaryReportDTO
    {
        [XMLProperty("user_id")]
        public int UserId { get; set; }
        [XMLProperty("user_name")]
        public string UserName { get; set; } = string.Empty;
        [XMLProperty("project_id")]
        public int ProjectId { get; set; }
        [XMLProperty("date")]
        public DateTime Date { get; set; }
        [XMLProperty("project_name")]
        public string ProjectName { get; set; } = string.Empty;
        [XMLProperty("task_id")]
        public int TaskId { get; set; }
        [XMLProperty("type")]
        public string Type { get; set; } = string.Empty;
        [XMLProperty("duration_in_minutes")]
        public int DurationInMinutes { get; set; }
    }
}
