using McgTgBotNet.DB.Entities;

namespace ReportBot.Services.Services.Interfaces;

public interface IUserService
{
    Task<User> AddUserAsync(User user);
    Task<User> GetUserByChatIdAsync(long chatId);
    Task<bool> UpdateUserShiftTimeAsync(long chatId, int shiftTime);
}
