using AutoMapper;
using McgTgBotNet.DB.Entities;
using McgTgBotNet.DTOs;
using McgTgBotNet.Extensions;
using McgTgBotNet.Models;
using Microsoft.EntityFrameworkCore;
using ReportBot.Common.DTOs;
using ReportBot.Common.Exceptions;
using ReportBot.Common.Requests;
using ReportBot.Common.Responses;
using ReportBot.DataBase.Entities;
using ReportBot.DataBase.Repositories.Interfaces;
using ReportBot.Services.Services.Interfaces;
using Telegram.Bot;

namespace ReportBot.Services.Services;

public class ReportService : IReportService
{
    private readonly IRepository<Report> _reportRepository;
    private readonly IRepository<Project> _projectRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IWorksnapsService _worksnapsService;
    private readonly TelegramBotClient _botClient;
    private readonly IMapper _mapper;

    public ReportService(
        IRepository<Report> reportRepository,
        IRepository<User> userRepository,
        IWorksnapsService worksnapsService,
        IMapper mapper,
        IRepository<Project> projectRepository)
    {
        _reportRepository = reportRepository;
        _userRepository = userRepository;
        _worksnapsService = worksnapsService;
        _mapper = mapper;
        _botClient = new TelegramBotClient(ConfigExtension.GetConfiguration("TelegramBot:Token"));
        _projectRepository = projectRepository;
    }

    public async Task<ReportDTO> AddReportAsync(CreateReportDTO report)
    {
        var user = await _userRepository.Include(x => x.Projects).FirstOrDefaultAsync(x => x.ChatId == report.ChatId)
            ?? throw new Exception("User not found");

        var project = user.Projects.FirstOrDefault(x => x.Name == report.ProjectName)
            ?? throw new Exception("Project not found");

        var entity = _mapper.Map<Report>(report);

        entity.UserId = user.Id;
        entity.ProjectId = project.Id;

        await _reportRepository.InsertAsync(entity);

        return _mapper.Map<ReportDTO>(entity);
    }

    public async Task<List<ReportDTO>> GetReportsForUserAsync(long chatId)
    {
        var user = await _userRepository.FirstOrDefaultAsync(x => x.ChatId == chatId)
            ?? throw new Exception("User not found");

        var reports = await _reportRepository
            .Include(x => x.Project)
            .Where(x => x.UserId == user.Id)
            .ToListAsync();

        return _mapper.Map<List<ReportDTO>>(reports);
    }

    public async Task<List<ReportDTO>> GetReportsAsync(FilterRequest filterRequest)
    {
        var query = _reportRepository
            .Include(x => x.Project)
            .Include(x => x.User)
            .AsQueryable();

        var reports = await FilterReportsAsync(query, filterRequest);

        return reports;
    }

    public async Task<List<ManagerReportResponse>> GetManagerReportAsync()
    {
        var summaryReports = await _worksnapsService.GetSummaryReportsAsync(DateTime.Today, DateTime.Today);

        var projects = await _projectRepository.ToListAsync();

        var result = new List<ManagerReportResponse>();

        foreach (var project in projects)
        {
            var query = _reportRepository
                .Include(x => x.User)
                .Where(x => x.ProjectId == project.Id)
                .Select(x => x.Message);

            if (query.ToList().Count == 0)
                continue;

            var item = new ManagerReportResponse
            {
                ProjectName = project.Name,
                TotalTime = summaryReports.Where(x => x.ProjectId == project.WorksnapsId).Sum(x => x.DurationInMinutes),
                Messages = await query.ToListAsync()
            };

            result.Add(item);
        }

        return result;
    }

    public async Task<List<ReportDTO>> GetReportsForProjectAsync(int projectId, FilterRequest filterRequest)
    {
        var query = _reportRepository
            .Include(x => x.Project)
            .Include(x => x.User)
            .Where(x => x.Project.Id == projectId)
            .AsQueryable();

        var reports = await FilterReportsAsync(query, filterRequest);

        return reports;
    }

    public async Task<SessionStatisticsResponse> GetSessionsStatiticsAsync()
    {
        var today = DateTime.Today;
        var totalSessions = await _worksnapsService.GetSummaryReportsAsync(today, today);

        var closeSessions = new List<SummaryReportDTO>();

        foreach (var item in totalSessions)
        {
            var lastTime = await _worksnapsService.GetLastTimeEntryAsync(item);
            if (lastTime == null)
                continue;

            var fromTime = DateTimeOffset.FromUnixTimeSeconds(lastTime.FromTimestamp).UtcDateTime.ToLocalTime();

            if (fromTime.AddMinutes(20) < DateTime.Now)
                closeSessions.Add(item);
        }

        var reportsToday = await _reportRepository
            .Where(x => x.DateOfShift.Date == DateTime.Now.Date)
            .ToListAsync();

        var result = new SessionStatisticsResponse
        {
            TotalSessions = totalSessions.Count,
            ClosedSessions = closeSessions.Count,
            OpenedSessions = totalSessions.Count - closeSessions.Count,
            EmployeesCount = totalSessions.Select(x => x.UserId).Distinct().Count(),
            ProjectsCount = totalSessions.Select(x => x.ProjectId).Distinct().Count(),
            ReportsCount = reportsToday.Count
        };

        return result;
    }

    public async Task<Dictionary<string, ReportStatisticsResponse>> GetReportsStatisticsAsync()
    {
        DateTime today = DateTime.Today;
        DateTime startOfWeek = today.AddDays(-(int)(today.DayOfWeek - DayOfWeek.Monday));

        if (today.DayOfWeek == DayOfWeek.Sunday)
            startOfWeek = today.AddDays(-6);

        DateTime endOfWeek = startOfWeek.AddDays(6);

        DateTime startOfMonth = new DateTime(today.Year, today.Month, 1);

        DateTime endOfMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month), 23, 59, 59);

        var dailySummary = await _worksnapsService.GetSummaryReportsAsync(today, today);

        var dailyStatistics = new ReportStatisticsResponse
        {
            TotalProjects = dailySummary.Select(x => x.ProjectName).Distinct().Count(),
            TotalReportsMinutes = dailySummary.Sum(x => x.DurationInMinutes)
        };

        var weeklySummary = await _worksnapsService.GetSummaryReportsAsync(startOfWeek, endOfWeek);

        var weeklyStatistics = new ReportStatisticsResponse
        {
            TotalProjects = weeklySummary.Select(x => x.ProjectName).Distinct().Count(),
            TotalReportsMinutes = weeklySummary.Sum(x => x.DurationInMinutes)
        };

        var monthlySummary = await _worksnapsService.GetSummaryReportsAsync(startOfMonth, endOfMonth);

        var monthlyStatistics = new ReportStatisticsResponse
        {
            TotalProjects = monthlySummary.Select(x => x.ProjectName).Distinct().Count(),
            TotalReportsMinutes = monthlySummary.Sum(x => x.DurationInMinutes)
        };


        var result = new Dictionary<string, ReportStatisticsResponse>
        {
            { "daily", dailyStatistics },
            { "weekly", weeklyStatistics },
            { "monthly", monthlyStatistics }
        };

        return result;
    }

    public async Task<bool> SendReportToChatAsync(int reportId)
    {
        var report = await _reportRepository
            .Include(x => x.Project)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == reportId)
            ?? throw new Exception("Report not found");

        if (report.Project.GroupId == null)
            throw new NotFoundException("Please, add chat to project");

        var text = $"💻 Project: {report.Project.Name}\n" +
                   $"👤 User: {report.User.Username}\n" +
                   $"📅 Date: {report.DateOfShift.Date:dd MMM yyyy}\n" +
                   $"📝 {report.Message}\n\n";

        var send = await _botClient.SendTextMessageAsync(report.Project.GroupId, text);

        return send != null;
    }

    private async Task<List<ReportDTO>> FilterReportsAsync(IQueryable<Report> query, FilterRequest request)
    {
        if (!string.IsNullOrEmpty(request.ProjectName))
        {
            query = query.Where(x => x.Project.Name == request.ProjectName);
        }

        if (request.FromDate != null)
        {
            query = query.Where(x => x.DateOfShift >= request.FromDate);
        }

        if (request.ToDate != null)
        {
            query = query.Where(x => x.DateOfShift <= request.ToDate);
        }

        if (!string.IsNullOrEmpty(request.UserName))
        {
            query = query.Where(x => x.UserName == request.UserName);
        }

        var reports = await query.ToListAsync();

        return _mapper.Map<List<ReportDTO>>(reports);
    }
}