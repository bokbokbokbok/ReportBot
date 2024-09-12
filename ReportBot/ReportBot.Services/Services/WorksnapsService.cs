using System.Net.Http.Headers;
using McgTgBotNet.DTOs;
using McgTgBotNet.Extensions;
using AutoMapper;
using System.Text.RegularExpressions;
using ReportBot.Services.Services.Interfaces;
using ReportBot.DataBase.Repositories.Interfaces;
using McgTgBotNet.DB.Entities;
using Microsoft.EntityFrameworkCore;
using ReportBot.Common.DTOs.Project;
using ReportBot.Services.Worksnaps;
using ReportBot.DataBase.Entities;

namespace McgTgBotNet.Services;

public class WorksnapsService : IWorksnapsService
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Project> _projectRepository;
    private readonly HttpClient _httpClient;
    private readonly IMapper _mapper;
    private readonly IWorksnapsRepository _worksnapsRepository;

    public WorksnapsService(
        IRepository<User> userRepository,
        IRepository<Project> projectRepository,
        IMapper mapper,
        IWorksnapsRepository worksnapsRepository)
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(ConfigExtension.GetConfiguration("Worksnaps:ApiKey"))));
        _userRepository = userRepository;
        _projectRepository = projectRepository;
        _mapper = mapper;
        _worksnapsRepository = worksnapsRepository;
    }

    public async Task<List<SummaryReportDTO>> GetFinishedReportsAsync()
    {
        var today = DateTime.Today;
        var data = await GetSummaryReportsAsync(today, today);
        var distinctData = data.GroupBy(x => new { x.UserId, x.ProjectId })
            .Select(x => x.First())
            .ToList();

        var result = new List<SummaryReportDTO>();

        foreach (var item in distinctData)
        {
            var project = await _projectRepository.FirstOrDefaultAsync(x => x.WorksnapsId == item.ProjectId);

            if (project == null || project.GroupId == null)
                continue;

            result.Add(item);
        }

        return await IsSessionFinishedAsync(result);
    }

    public async Task<List<SummaryReportDTO>> GetSummaryReportsAsync(DateTime from, DateTime to)
    {
        var data = await _worksnapsRepository.GetSummaryReportsAsync(null, from, to, null);

        return data;
    }

    public async Task<WorksnapsUserDTO> GetUserAsync(string email)
    {
        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase))
            throw new ArgumentException("Email is not valid");

        var data = await _worksnapsRepository.GetUsersAsync(null);

        var user = data.FirstOrDefault(x => x.Email == email);

        if (user == null)
            throw new ArgumentException($"😔 No user with this email was found. Email: {email}");

        return user;
    }

    public async Task<WorksnapsUserDTO> GetUserByWorksnapsId(int id)
    {
        var user = await _worksnapsRepository.GetUserByIdAsync(null, id);

        return user;
    }

    public async Task<List<SummaryReportDTO>> GetSummaryReportsForProjectAsync(int projectId, DateTime from, DateTime to)
    {
        var data = await _worksnapsRepository.GetSummaryReportsAsync(null, from, to, projectId.ToString());

        return data;
    }

    public async Task<List<ProjectDTO>> GetWorksnapsProjectsAsync(int userId)
    {
        var userWorksnaps = await GetUserByWorksnapsId(userId);

        var data = await _worksnapsRepository.GetProjectsAsync(userWorksnaps.ApiToken);

        var assignments = new List<AssignmentDTO>();
        foreach (var item in data)
        {
            var assignment = await _worksnapsRepository.GetUserAssignmentAsync(userWorksnaps.ApiToken, item.WorksnapsId.ToString());

            assignments.AddRange(assignment);
        }

        var result = data.Where(x => assignments.Any(a => a.UserId == userId && a.Role == "Manager" && x.WorksnapsId == a.ProjectId)).ToList();

        return result;
    }

    public async Task<bool> AddProjectToUser(int userId)
    {
        var userWorksnaps = await GetUserByWorksnapsId(userId);

        var data = await _worksnapsRepository.GetProjectsAsync(userWorksnaps.ApiToken);

        var projects = new List<Project>();

        foreach (var item in _mapper.Map<List<Project>>(data))
        {
            var project = await _projectRepository.FirstOrDefaultAsync(x => x.Name == item.Name);
            if (project == null)
            {
                await _projectRepository.InsertAsync(item);
                projects.Add(item);
            }
            else
            {
                projects.Add(project);
            }
        }
        var user = await _userRepository.FirstOrDefaultAsync(x => x.WorksnapsId == userId)
            ?? throw new Exception("User not found");

        user.Projects.AddRange(projects);

        var result = await _userRepository.UpdateAsync(user);

        return result;
    }

    public async Task<string> GetUserRoleAsync(int id)
    {
        var userWorksnaps = await GetUserByWorksnapsId(id);

        var projects = await _worksnapsRepository.GetProjectsAsync(userWorksnaps.ApiToken);

        var assignments = await _worksnapsRepository.GetUserAssignmentAsync(userWorksnaps.ApiToken, projects.First().WorksnapsId.ToString());

        var user = assignments.FirstOrDefault(x => x.UserId == id)
            ?? throw new Exception("Not found user");

        return user.Role;
    }

    public async Task<TimeEntryDTO> GetLastTimeEntryAsync(SummaryReportDTO dto)
    {
        var startOfDay = DateTime.Today.Date;
        var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

        var fromTimestamp = new DateTimeOffset(startOfDay, TimeZoneInfo.Local.GetUtcOffset(startOfDay)).ToUnixTimeSeconds();
        var toTimestamp = new DateTimeOffset(endOfDay, TimeZoneInfo.Local.GetUtcOffset(endOfDay)).ToUnixTimeSeconds();

        var data = await _worksnapsRepository.GetTimeEntriesAsync(null, dto.ProjectId.ToString(), dto.UserId.ToString(), fromTimestamp, toTimestamp);

        return data.Last();
    }

    public async Task<List<TimeEntryDTO>> GetTimeEntryAsync(int userId, int projectId, DateTime date)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

        var fromTimestamp = new DateTimeOffset(startOfDay, TimeZoneInfo.Local.GetUtcOffset(startOfDay)).ToUnixTimeSeconds();
        var toTimestamp = new DateTimeOffset(endOfDay, TimeZoneInfo.Local.GetUtcOffset(endOfDay)).ToUnixTimeSeconds();

        var data = await _worksnapsRepository.GetTimeEntriesAsync(null, projectId.ToString(), userId.ToString(), fromTimestamp, toTimestamp);

        return data;
    }

    private async Task<List<SummaryReportDTO>> IsSessionFinishedAsync(List<SummaryReportDTO> data)
    {
        var result = new List<SummaryReportDTO>();

        foreach (var item in data)
        {
            var user = await _userRepository.Include(x => x.Reports).FirstOrDefaultAsync(x => x.WorksnapsId == item.UserId);


            if (user == null || user.Role == "manager")
                continue;

            var lastReport = user.Reports.OrderByDescending(x => x.Created).FirstOrDefault();

            if (lastReport == null || lastReport.Created.AddMinutes(20) >= DateTime.Now)
                continue;

            if (await GetTimeEntryAsync(item))
                result.Add(item);
        }

        return result;
    }

    private async Task<bool> GetTimeEntryAsync(SummaryReportDTO dto)
    {
        var lastTimeEntry = await GetLastTimeEntryAsync(dto);
        var loggedTime = DateTimeOffset.FromUnixTimeSeconds(lastTimeEntry.LoggedTimestamp).UtcDateTime.ToLocalTime();

        if (!(loggedTime.AddMinutes(15) > DateTime.Now)
            && loggedTime.AddMinutes(10) >= DateTime.Now.AddMinutes(-10)
            && loggedTime.AddMinutes(10) <= DateTime.Now)
        {
            return true;
        }

        return false;
    }
}