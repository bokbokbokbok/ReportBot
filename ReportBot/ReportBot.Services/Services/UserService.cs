using McgTgBotNet.DB.Entities;
using Microsoft.EntityFrameworkCore;
using ReportBot.DataBase.Repositories.Interfaces;
using ReportBot.Services.Services.Interfaces;

namespace ReportBot.Services.Services;

public class UserService : IUserService
{
    private readonly IRepository<User> _userRepository;

    public UserService(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> AddUserAsync(User user)
    {
        var entity = await _userRepository.FirstOrDefaultAsync(p => p.WorksnapsId == user.WorksnapsId);

        if (entity != null)
            throw new Exception($"👋 Hi {user.Username}, you are already registered");

        await _userRepository.InsertAsync(user);

        return user;
    }

    public async Task<bool> UpdateUserShiftTimeAsync(long chatId, int shiftTime)
    {
        var user = await _userRepository.FirstOrDefaultAsync(x => x.ChatId == chatId)
            ?? throw new Exception("User not found");

        user.ShiftTime = shiftTime;

        var result = await _userRepository.UpdateAsync(user);

        return result;
    }

    public async Task<User> GetUserByChatIdAsync(long chatId)
    {
        var user = await _userRepository
            .Include(x => x.Projects)
            .FirstOrDefaultAsync(x => x.ChatId == chatId)
            ?? throw new Exception("User not found");

        return user;
    }
}
