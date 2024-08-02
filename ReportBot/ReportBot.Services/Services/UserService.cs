using AutoMapper;
using McgTgBotNet.DB.Entities;
using McgTgBotNet.DTOs;
using McgTgBotNet.Extensions;
using Microsoft.EntityFrameworkCore;
using ReportBot.Common.Enums;
using ReportBot.Common.Exceptions;
using ReportBot.DataBase.Repositories.Interfaces;
using ReportBot.Services.Services.Interfaces;
using System.Net.Http.Headers;
using System.Xml.Linq;

namespace ReportBot.Services.Services;

public class UserService : IUserService
{
    private readonly IRepository<User> _userRepository;
    private readonly IWorksnapsService _worksnapsService;
    private readonly HttpClient _httpClient;

    public UserService(
        IRepository<User> userRepository,
        IWorksnapsService worksnapsService)
    {
        _userRepository = userRepository;
        _worksnapsService = worksnapsService;
        _httpClient = new HttpClient();
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

    public async Task<List<WorksnapsUserDTO>> GetUsersAsync(int managerId, SortingEnum sorting)
    {
        var manager = await _userRepository
            .Include(x => x.Projects)
            .ThenInclude(x => x.Users)
            .FirstOrDefaultAsync(x => x.Id == managerId)
            ?? throw new Exception("Manager not found");

        var users = await GetUsersFromWorksnapsAsync(manager);

        var result = SortUsers(users, sorting);

        return result;
    }

    private async Task<List<WorksnapsUserDTO>> GetUsersFromWorksnapsAsync(User manager)
    {
        var userWorksnaps = await _worksnapsService.GetUserByWorksnapsId(manager.WorksnapsId);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(userWorksnaps.ApiToken)));

        var response = await _httpClient.GetAsync($"https://api.worksnaps.com:443/api/users.xml");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        var doc = XDocument.Parse(content);

        var data = new List<WorksnapsUserDTO>();

        foreach (var element in doc.Root!.Elements())
        {
            var item = element.ParseXML<WorksnapsUserDTO>();

            data.Add(item);
        }

        return data;
    }

    private List<WorksnapsUserDTO> SortUsers(List<WorksnapsUserDTO> users, SortingEnum sorting)
    {
        users = sorting switch
        {
            SortingEnum.None => users,
            SortingEnum.FirstName => users.OrderBy(x => x.FirstName).ToList(),
            SortingEnum.LastName => users.OrderBy(x => x.LastName).ToList(),
            SortingEnum.Username => users.OrderBy(x => x.Login).ToList(),
            SortingEnum.Email => users.OrderBy(x => x.Email).ToList(),
            _ => throw new IncorrectParametersException("Incorrect sorting parameter")
        };

        return users;
    }
}