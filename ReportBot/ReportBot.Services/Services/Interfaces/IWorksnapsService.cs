
using McgTgBotNet.DTOs;

namespace ReportBot.Services.Services.Interfaces;

public interface IWorksnapsService
{
    Task<bool> AddProjectToUser(int userId);
    Task<List<SummaryReportDTO>> GetSummaryReportsAsync();
    Task<UserDTO> GetUserByWorksnapsId(int id);
    Task<int> GetUserId(string email);
    Task<string> GetUserRoleAsync(int id);
}
