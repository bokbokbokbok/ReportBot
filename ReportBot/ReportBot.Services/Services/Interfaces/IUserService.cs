using McgTgBotNet.DB.Entities;
using ReportBot.Common.Responses;

namespace ReportBot.Services.Services.Interfaces;

public interface IUserService
{
    Task<User> AddUserAsync(User user);
    Task<User> GetUserByChatIdAsync(long chatId);
    Task<List<UserResponse>> GetUsersAsync(int managerId, string? projectName);
}