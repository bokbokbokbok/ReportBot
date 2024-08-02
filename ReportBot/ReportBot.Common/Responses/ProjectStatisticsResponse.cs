using ReportBot.Common.DTOs.Project;

namespace ReportBot.Common.Responses;

public class ProjectStatisticsResponse
{
    public ProjectDTO Project { get; set; } = new();
    public int TotalMinutesPerWeek { get; set; }
    public int TotalMinutesPerDay { get; set; }
}