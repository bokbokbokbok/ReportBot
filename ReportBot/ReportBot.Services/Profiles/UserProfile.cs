using AutoMapper;
using McgTgBotNet.DB.Entities;
using McgTgBotNet.DTOs;
using ReportBot.Common.DTOs;

namespace McgTgBotNet.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, WorksnapsUserDTO>();
        CreateMap<User, UserDTO>();
    }
}
