using AutoMapper;
using McgTgBotNet.DB.Entities;
using McgTgBotNet.Models;
using Microsoft.EntityFrameworkCore;
using ReportBot.Common.DTOs;
using ReportBot.DataBase.Repositories.Interfaces;
using ReportBot.Services.Services.Interfaces;

namespace ReportBot.Services.Services;

public class ReportService : IReportService
{
    private readonly IRepository<Report> _reportRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IMapper _mapper;

    public ReportService(
        IRepository<Report> reportRepository,
        IMapper mapper,
        IRepository<User> userRepository)
    {
        _reportRepository = reportRepository;
        _mapper = mapper;
        _userRepository = userRepository;
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
}
