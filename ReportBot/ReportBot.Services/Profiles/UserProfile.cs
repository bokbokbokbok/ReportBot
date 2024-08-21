using AutoMapper;
using McgTgBotNet.DB.Entities;
using McgTgBotNet.DTOs;
using ReportBot.Common.DTOs;

namespace McgTgBotNet.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, WorksnapsUserDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.WorksnapsId))
            .ForMember(dest => dest.Login, opt => opt.MapFrom(src => src.Username));
        CreateMap<User, UserDTO>();
    }
}
