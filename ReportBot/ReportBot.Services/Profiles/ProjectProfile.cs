using AutoMapper;
using McgTgBotNet.Models;
using ReportBot.Common.DTOs.Project;

namespace McgTgBotNet.Profiles;

public class ProjectProfile : Profile
{
    public ProjectProfile()
    {
        CreateMap<ProjectDTO, Project>();
        CreateMap<Project, ProjectDTO>();
    }
}