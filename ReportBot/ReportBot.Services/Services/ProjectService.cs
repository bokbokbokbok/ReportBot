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
        var query = _projectRepository
            .Include(x => x.Users)
            .Where(p => p.Users.Any(u => u.Id == userId));
        
        var result = _mapper.Map<List<ProjectDTO>>(await query.ToListAsync());

        return result;
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

        var query = _projectRepository
            .Include(x => x.Users)
            .Include(x => x.Reports)
            .Where(p => p.Users.Any(u => u.Id == userId));

        var result = new List<ProjectStatisticsResponse>();

        foreach (var project in await query.ToListAsync())
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