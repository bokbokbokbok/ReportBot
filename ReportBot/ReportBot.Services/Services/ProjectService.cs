using AutoMapper;
using McgTgBotNet.Models;
using Microsoft.EntityFrameworkCore;
using ReportBot.Common.DTOs.Project;
using ReportBot.DataBase.Repositories.Interfaces;
using ReportBot.Services.Services.Interfaces;

namespace ReportBot.Services.Services;

public class ProjectService : IProjectService
{
    private readonly IRepository<Project> _projectRepository;
    private readonly IMapper _mapper;

    public ProjectService(IRepository<Project> projectRepository, IMapper mapper)
    {
        _projectRepository = projectRepository;
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
}