using McgTgBotNet.DB.Entities;
using McgTgBotNet.DTOs;
using ReportBot.Common.Enums;

namespace ReportBot.Services.Services.Interfaces;

public interface IUserService
{
    Task<User> AddUserAsync(User user);
    Task<User> GetUserByChatIdAsync(long chatId);
    Task<List<WorksnapsUserDTO>> GetUsersAsync(int managerId, SortingEnum sorting);
}