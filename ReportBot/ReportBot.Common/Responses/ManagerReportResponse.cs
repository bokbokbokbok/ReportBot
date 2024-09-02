namespace ReportBot.Common.Responses
{
    public class ManagerReportResponse
    {
        public string ProjectName { get; set; } = string.Empty;
        public int TotalTime { get; set; }
        public List<string> Messages { get; set; } = new();
    }
}
