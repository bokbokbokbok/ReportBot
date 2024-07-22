using AutoMapper;
using McgTgBotNet.DTOs;
using McgTgBotNet.Models;

namespace McgTgBotNet.Profiles
{
    public class ProjectProfile : Profile
    {
        public ProjectProfile()
        {
            CreateMap<Project, ProjectDTO>();
        }
    }
}
