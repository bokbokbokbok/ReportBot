using McgTgBotNet.Models;
using ReportBot.Common.DTOs.Project;
using ReportBot.Common.Responses;

namespace ReportBot.Services.Services.Interfaces;

public interface IProjectService
{
    Task<List<ProjectStatisticsResponse>> GetProjectStatistics(int userId);
    Task<ProjectDTO> GetProjectByIdAsync(int id);
    Task<List<ProjectDTO>> GetProjectsAsync(int userId);
}
