namespace ReportBot.Common.Responses;

public class SessionStatisticsResponse
{
    public int TotalSessions { get; set; }
    public int ClosedSessions { get; set; }
    public int OpenedSessions { get; set; }
    public int EmployeesCount { get; set; }
    public int ProjectsCount { get; set; }
    public int ReportsCount { get; set; }
}