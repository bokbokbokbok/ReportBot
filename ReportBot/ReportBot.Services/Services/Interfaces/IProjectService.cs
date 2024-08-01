using McgTgBotNet.Models;
using ReportBot.Common.DTOs.Project;

namespace ReportBot.Services.Services.Interfaces;

public interface IProjectService
{
    Task<ProjectDTO> GetProjectByIdAsync(int id);
    Task<List<ProjectDTO>> GetProjectsAsync(int userId);
}
