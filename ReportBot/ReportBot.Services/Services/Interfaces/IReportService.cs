using ReportBot.Common.DTOs;

namespace ReportBot.Services.Services.Interfaces
{
    public interface IReportService
    {
        Task<ReportDTO> AddReportAsync(CreateReportDTO report);
    }
}
