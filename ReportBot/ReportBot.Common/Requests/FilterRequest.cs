namespace ReportBot.Common.Requests;

public class FilterRequest
{
    public string? ProjectName { get; set; } = string.Empty;
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? UserName { get; set; } = string.Empty;
}