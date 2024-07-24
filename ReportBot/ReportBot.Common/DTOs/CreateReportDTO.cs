namespace ReportBot.Common.DTOs;

public class CreateReportDTO
{
    public DateTime Created { get; set; }
    public string Message { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime DateOfShift { get; set; }
    public int TimeOfShift { get; set; }
    public long ChatId { get; set; }
    public int UserId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}   
