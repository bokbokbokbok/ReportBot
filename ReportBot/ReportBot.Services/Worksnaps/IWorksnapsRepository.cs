using McgTgBotNet.DTOs;
using ReportBot.Common.DTOs.Project;

namespace ReportBot.Services.Worksnaps
{
    public interface IWorksnapsRepository
    {
        Task<List<ProjectDTO>> GetProjectsAsync(string token);
        Task<List<SummaryReportDTO>> GetSummaryReportsAsync(string? token, DateTime from, DateTime to, string? projectIds);
        Task<List<TimeEntryDTO>> GetTimeEntriesAsync(string? token, string projectId, string userId, long fromTimestamp, long toTimestamp);
        Task<List<AssignmentDTO>> GetUserAssignmentAsync(string? token, string projectIds);
        Task<WorksnapsUserDTO> GetUserByIdAsync(string? token, int id);
        Task<List<WorksnapsUserDTO>> GetUsersAsync(string? token);
    }
}
