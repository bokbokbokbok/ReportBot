
using McgTgBotNet.DTOs;
using ReportBot.Common.DTOs.Project;

namespace ReportBot.Services.Services.Interfaces;

public interface IWorksnapsService
{
    Task<bool> AddProjectToUser(int userId);
    Task<List<SummaryReportDTO>> GetFinishedReportsAsync();
    Task<List<SummaryReportDTO>> GetSummaryReportsAsync(DateTime from, DateTime to);
    Task<List<SummaryReportDTO>> GetSummaryReportsForProjectAsync(int projectId, DateTime from, DateTime to);
    Task<WorksnapsUserDTO> GetUserByWorksnapsId(int id);
    Task<WorksnapsUserDTO> GetUserAsync(string email);
    Task<string> GetUserRoleAsync(int id);
    Task<List<ProjectDTO>> GetWorksnapsProjectsAsync(int userId);
    Task<TimeEntryDTO> GetLastTimeEntryAsync(SummaryReportDTO dto);
}
