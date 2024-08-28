using AutoMapper;
using McgTgBotNet.Models;
using Microsoft.EntityFrameworkCore;
using ReportBot.Common.DTOs.Project;
using ReportBot.Common.Responses;
using ReportBot.DataBase.Repositories.Interfaces;
using ReportBot.Services.Services.Interfaces;

namespace ReportBot.Services.Services;

public class ProjectService : IProjectService
{
    private readonly IRepository<Project> _projectRepository;
    private readonly IWorksnapsService _worksnapsService;
    private readonly IMapper _mapper;

    public ProjectService(
        IRepository<Project> projectRepository,
        IWorksnapsService worksnapsService,
        IMapper mapper)
    {
        _projectRepository = projectRepository;
        _worksnapsService = worksnapsService;
        _mapper = mapper;
    }

    public async Task<List<ProjectDTO>> GetProjectsAsync(int userId)
    {
        var worksnapsProjects = await _worksnapsService.GetWorksnapsProjectsAsync(userId);

        var projects = new List<Project>();

        foreach (var project in worksnapsProjects)
        {
            var item = await _projectRepository
                .Include(x => x.Users)
                .FirstOrDefaultAsync(x => x.WorksnapsId == project.WorksnapsId);

            if (item == null)       
                continue;

            projects.Add(item);
        }

        return _mapper.Map<List<ProjectDTO>>(projects);
    }

    public async Task<ProjectDTO> GetProjectByIdAsync(int id)
    {
        var project = await _projectRepository
            .Include(x => x.Users)
            .Include(x => x.Reports)
            .FirstOrDefaultAsync(x => x.Id == id);

        var result = _mapper.Map<ProjectDTO>(project);

        return result;
    }

    public async Task<List<ProjectStatisticsResponse>> GetProjectStatistics(int userId)
    {
        DateTime today = DateTime.Today;
        DateTime startOfWeek = today.AddDays(-(int)(today.DayOfWeek - DayOfWeek.Monday));

        if (today.DayOfWeek == DayOfWeek.Sunday)
            startOfWeek = today.AddDays(-6);

        DateTime endOfWeek = startOfWeek.AddDays(6);

        var projects = await _worksnapsService.GetWorksnapsProjectsAsync(userId);
        var user = await _worksnapsService.GetUserByWorksnapsId(userId);
        var result = new List<ProjectStatisticsResponse>();

        foreach (var project in projects)
        {
            var daylySummary = await _worksnapsService.GetSummaryReportsForProjectAsync(project.WorksnapsId, today.Date, today.Date);
            var weeklySummary = await _worksnapsService.GetSummaryReportsForProjectAsync(project.WorksnapsId, startOfWeek.Date, endOfWeek.Date);

            var projectStatistics = new ProjectStatisticsResponse
            {
                Project = _mapper.Map<ProjectDTO>(project),
                TotalMinutesPerWeek = weeklySummary.Sum(x => x.DurationInMinutes),
                TotalMinutesPerDay = daylySummary.Sum(x => x.DurationInMinutes),
            };

            result.Add(projectStatistics);
        }

        return result;
    }
}