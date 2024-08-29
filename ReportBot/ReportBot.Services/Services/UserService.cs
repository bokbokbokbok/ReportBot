using AutoMapper;
using McgTgBotNet.DB.Entities;
using Microsoft.EntityFrameworkCore;
using ReportBot.Common.DTOs;
using ReportBot.Common.Responses;
using ReportBot.DataBase.Entities;
using ReportBot.DataBase.Repositories.Interfaces;
using ReportBot.Services.Services.Interfaces;

namespace ReportBot.Services.Services;

public class UserService : IUserService
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Project> _projectRepository;
    private readonly IWorksnapsService _worksnapsService;
    private readonly IMapper _mapper;

    public UserService(
        IRepository<User> userRepository,
        IWorksnapsService worksnapsService,
        IRepository<Project> projectRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _worksnapsService = worksnapsService;
        _projectRepository = projectRepository;
        _mapper = mapper;
    }

    public async Task<User> AddUserAsync(User user)
    {
        var entity = await _userRepository.FirstOrDefaultAsync(p => p.WorksnapsId == user.WorksnapsId);

        if (entity != null)
            throw new Exception($"👋 Hi {user.Username}, you are already registered");

        await _userRepository.InsertAsync(user);

        return user;
    }

    public async Task<User> GetUserByChatIdAsync(long chatId)
    {
        var user = await _userRepository
            .Include(x => x.Projects)
            .FirstOrDefaultAsync(x => x.ChatId == chatId)
            ?? throw new Exception("User not found");

        return user;
    }

    public async Task<List<UserResponse>> GetUsersAsync(int managerId, string? projectName)
    {
        DateTime today = DateTime.Today;
        DateTime startOfWeek = today.AddDays(-(today.DayOfWeek - DayOfWeek.Monday));

        if (today.DayOfWeek == DayOfWeek.Sunday)
            startOfWeek = today.AddDays(-6);

        DateTime endOfWeek = startOfWeek.AddDays(6);

        var projectUsers = await GetUsersForProjectsAsync(managerId);
        var filteredUsers = FilteredUsers(projectUsers, projectName);

        var users = new List<UserResponse>();

        var summaryReportPerDay = await _worksnapsService.GetSummaryReportsAsync(today, today);
        var summaryReportPerWeek = await _worksnapsService.GetSummaryReportsAsync(startOfWeek, endOfWeek);

        foreach (var item in filteredUsers)
        {
            var user = new UserResponse
            {
                User = _mapper.Map<UserDTO>(item),
                TimePerDay = summaryReportPerDay.Where(x => x.UserId == item.WorksnapsId).Sum(x => x.DurationInMinutes),
                TimePerWeek = summaryReportPerWeek.Where(x => x.UserId == item.WorksnapsId).Sum(x => x.DurationInMinutes)
            };

            users.Add(user);
        }

        return users;
    }

    private async Task<List<User>> GetUsersForProjectsAsync(int managerId)
    {
        var projects = await _worksnapsService.GetWorksnapsProjectsAsync(managerId);
        var users = new List<User>();

        foreach (var item in projects)
        {
            var project = await _projectRepository.Include(x => x.Users).FirstOrDefaultAsync(x => x.Name == item.Name);

            if (project == null)
                continue;
            users.AddRange(project.Users);
        }

        return users;
    }

    private List<User> FilteredUsers(List<User> users, string? projectName)
    {
        if (string.IsNullOrEmpty(projectName))
            return users;

        var result = users.Where(x => x.Projects.Any(p => p.Name.ToLower() == projectName.ToLower())).ToList();

        return result;
    }
}