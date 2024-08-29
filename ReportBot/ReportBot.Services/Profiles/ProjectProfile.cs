using AutoMapper;
using ReportBot.Common.DTOs.Project;
using ReportBot.DataBase.Entities;

namespace McgTgBotNet.Profiles;

public class ProjectProfile : Profile
{
    public ProjectProfile()
    {
        CreateMap<ProjectDTO, Project>();
        CreateMap<Project, ProjectDTO>();
    }
}