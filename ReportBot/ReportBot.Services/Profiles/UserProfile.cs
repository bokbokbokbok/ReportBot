using AutoMapper;
using McgTgBotNet.DB.Entities;
using McgTgBotNet.DTOs;

namespace McgTgBotNet.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDTO > ();
    }
}
