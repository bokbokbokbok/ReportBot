
using McgTgBotNet.DTOs;

namespace ReportBot.Services.Services.Interfaces;

public interface IWorksnapsService
{
    Task<bool> AddProjectToUser(int userId);
    Task<Dictionary<int, bool>> GetSummaryReportsAsync();
    Task<UserDTO> GetUserByWorksnapsId(int id);
    Task<int> GetUserId(string email);
}
