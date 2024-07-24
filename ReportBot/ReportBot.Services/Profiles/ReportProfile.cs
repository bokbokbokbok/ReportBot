using AutoMapper;
using McgTgBotNet.Models;
using ReportBot.Common.DTOs;

namespace McgTgBotNet.Profiles;

public class ReportProfile : Profile
{
    public ReportProfile()
    {
        CreateMap<CreateReportDTO, Report>();
        CreateMap<Report, ReportDTO > ();
    }
}
